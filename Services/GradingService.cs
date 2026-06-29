using ExaminationSystem.Data;
using ExaminationSystem.Dtos;
using ExaminationSystem.Models;
using ExaminationSystem.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Services
{
    public class GradingService
    {
        private readonly Context _context;

        public GradingService(Context context)
        {
            _context = context;
        }

        public async Task<ExamResultDto> SubmitExamAsync(int studentId, int examId, SubmitExamDto dto)
        {
            var examStudent = await _context.ExamStudents
                .Include(es => es.Exam)
                .FirstOrDefaultAsync(es => es.ExamId == examId
                    && es.StudentId == studentId
                    && !es.IsDeleted);

            if (examStudent == null)
                throw new InvalidOperationException("You are not assigned to this exam.");

            if (examStudent.IsSubmitted)
                throw new InvalidOperationException("Exam already submitted.");

            var exam = examStudent.Exam;
            if (exam.IsDeleted)
                throw new InvalidOperationException("Exam not found.");

            if (exam.ExamType == ExamType.Final)
            {
                var hasFinal = await _context.ExamStudents
                    .AnyAsync(es => es.StudentId == studentId
                        && es.IsSubmitted
                        && !es.IsDeleted
                        && es.Exam!.ExamType == ExamType.Final
                        && es.Exam.CourseId == exam.CourseId
                        && es.ExamId != examId);

                if (hasFinal)
                    throw new InvalidOperationException("You can only take one final exam per course.");
            }

            var examQuestions = await _context.ExamQuestions
                .Where(eq => eq.ExamId == examId && !eq.IsDeleted)
                .Select(eq => eq.QuestionId)
                .ToListAsync();

            if (dto.Answers.Count != examQuestions.Count)
                throw new InvalidOperationException("All exam questions must be answered.");

            var questionIds = examQuestions.ToHashSet();
            if (dto.Answers.Any(a => !questionIds.Contains(a.QuestionId)))
                throw new InvalidOperationException("Invalid question in submission.");

            var choices = await _context.Choices
                .Where(c => !c.IsDeleted && examQuestions.Contains(c.QuestionId))
                .ToListAsync();

            var existingAnswers = await _context.StudentAnswers
                .Where(sa => sa.StudentId == studentId && sa.ExamId == examId)
                .ToListAsync();

            _context.StudentAnswers.RemoveRange(existingAnswers);

            var correctCount = 0;
            foreach (var answer in dto.Answers)
            {
                var choice = choices.FirstOrDefault(c =>
                    c.Id == answer.ChoiceId && c.QuestionId == answer.QuestionId);

                if (choice == null)
                    throw new InvalidOperationException("Invalid choice for a question.");

                var isCorrect = choice.IsCorrect;
                if (isCorrect) correctCount++;

                _context.StudentAnswers.Add(new StudentAnswer
                {
                    StudentId = studentId,
                    ExamId = examId,
                    QuestionId = answer.QuestionId,
                    ChoiceId = answer.ChoiceId,
                    CourseId = exam.CourseId,
                    IsCorrect = isCorrect,
                    CreatedAt = DateTime.UtcNow
                });
            }

            var total = examQuestions.Count;
            var score = total > 0 ? Math.Round((double)correctCount / total * 100, 2) : 0;

            examStudent.IsSubmitted = true;
            examStudent.SubmittedAt = DateTime.UtcNow;
            examStudent.CorrectAnswers = correctCount;
            examStudent.TotalQuestions = total;
            examStudent.Score = score;

            await _context.SaveChangesAsync();

            var student = await _context.Students.FindAsync(studentId);

            return new ExamResultDto
            {
                ExamId = examId,
                ExamName = exam.Name,
                StudentId = studentId,
                StudentName = student?.Name ?? string.Empty,
                CorrectAnswers = correctCount,
                TotalQuestions = total,
                Score = score,
                SubmittedAt = examStudent.SubmittedAt
            };
        }
    }
}
