using ExaminationSystem.Data;
using ExaminationSystem.Models;
using ExaminationSystem.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Services
{
    public class ExamAssignmentService
    {
        private readonly Context _context;

        public ExamAssignmentService(Context context)
        {
            _context = context;
        }

        public async Task<List<Question>> AssignQuestionsAsync(Exam exam, List<int>? manualQuestionIds)
        {
            var existing = await _context.ExamQuestions
                .Where(eq => eq.ExamId == exam.Id)
                .ToListAsync();

            _context.ExamQuestions.RemoveRange(existing);

            List<Question> selected;

            if (exam.IsAutoAssign)
                selected = await AutoAssignQuestionsAsync(exam);
            else
                selected = await ManualAssignQuestionsAsync(exam, manualQuestionIds ?? new List<int>());

            foreach (var question in selected)
            {
                _context.ExamQuestions.Add(new ExamQuestion
                {
                    ExamId = exam.Id,
                    QuestionId = question.Id,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return selected;
        }

        private async Task<List<Question>> AutoAssignQuestionsAsync(Exam exam)
        {
            var pool = await _context.Questions
                .Where(q => q.CourseId == exam.CourseId
                    && q.InstructorId == exam.InstructorId
                    && !q.IsDeleted)
                .ToListAsync();

            if (pool.Count < exam.QuestionCount)
                throw new InvalidOperationException(
                    $"Not enough questions in the course bank. Required: {exam.QuestionCount}, Available: {pool.Count}");

            var simpleCount = exam.QuestionCount / 3;
            var mediumCount = exam.QuestionCount / 3;
            var hardCount = exam.QuestionCount - simpleCount - mediumCount;

            var simple = PickRandom(pool.Where(q => q.QuestionLevel == QuestionLevel.Simple), simpleCount);
            var medium = PickRandom(pool.Where(q => q.QuestionLevel == QuestionLevel.Medium), mediumCount);
            var hard = PickRandom(pool.Where(q => q.QuestionLevel == QuestionLevel.Hard), hardCount);

            var selected = simple.Concat(medium).Concat(hard).ToList();

            if (selected.Count < exam.QuestionCount)
            {
                var usedIds = selected.Select(q => q.Id).ToHashSet();
                var remaining = PickRandom(pool.Where(q => !usedIds.Contains(q.Id)),
                    exam.QuestionCount - selected.Count);
                selected.AddRange(remaining);
            }

            return selected;
        }

        private async Task<List<Question>> ManualAssignQuestionsAsync(Exam exam, List<int> questionIds)
        {
            if (questionIds.Count != exam.QuestionCount)
                throw new InvalidOperationException(
                    $"Manual assignment requires exactly {exam.QuestionCount} questions.");

            var questions = await _context.Questions
                .Where(q => questionIds.Contains(q.Id)
                    && q.CourseId == exam.CourseId
                    && q.InstructorId == exam.InstructorId
                    && !q.IsDeleted)
                .ToListAsync();

            if (questions.Count != exam.QuestionCount)
                throw new InvalidOperationException("One or more questions are invalid or do not belong to this course.");

            return questions;
        }

        private static List<Question> PickRandom(IEnumerable<Question> source, int count)
        {
            return source.OrderBy(_ => Guid.NewGuid()).Take(count).ToList();
        }
    }
}
