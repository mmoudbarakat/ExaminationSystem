using System.Security.Claims;
using ExaminationSystem.Data;
using ExaminationSystem.Dtos;
using ExaminationSystem.Models.Enums;
using ExaminationSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Controllers
{
    [ApiController]
    [Route("api/student-exams")]
    [Authorize(Roles = "Student")]
    public class StudentExamController : ControllerBase
    {
        private readonly Context _context;
        private readonly GradingService _gradingService;

        public StudentExamController(Context context, GradingService gradingService)
        {
            _context = context;
            _gradingService = gradingService;
        }

        private int StudentId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<ActionResult<List<StudentExamDto>>> GetMyExams()
        {
            var exams = await _context.ExamStudents
                .Where(es => es.StudentId == StudentId && !es.IsDeleted)
                .Include(es => es.Exam)
                .Select(es => new StudentExamDto
                {
                    ExamId = es.ExamId,
                    ExamName = es.Exam!.Name,
                    ExamType = es.Exam.ExamType,
                    IsSubmitted = es.IsSubmitted,
                    Score = es.Score
                })
                .ToListAsync();

            return Ok(exams);
        }

        [HttpGet("{examId}")]
        public async Task<ActionResult<object>> GetExamQuestions(int examId)
        {
            var assignment = await _context.ExamStudents
                .Include(es => es.Exam)
                .FirstOrDefaultAsync(es => es.ExamId == examId && es.StudentId == StudentId && !es.IsDeleted);

            if (assignment == null) return NotFound("Exam not assigned to you.");

            if (assignment.IsSubmitted) return BadRequest("Exam already submitted.");

            var exam = assignment.Exam!;
            var questions = await _context.ExamQuestions
                .Where(eq => eq.ExamId == examId && !eq.IsDeleted)
                .Include(eq => eq.Question!)
                    .ThenInclude(q => q.Choices.Where(c => !c.IsDeleted))
                .Select(eq => eq.Question!)
                .ToListAsync();

            return Ok(new
            {
                examId,
                examName = exam.Name,
                examType = exam.ExamType,
                durationMinutes = exam.Duration.TotalMinutes,
                questions = questions.Select(q => new QuestionForStudentDto
                {
                    Id = q.Id,
                    Header = q.Header,
                    QuestionLevel = q.QuestionLevel,
                    Choices = q.Choices.Select(c => new ChoiceForStudentDto
                    {
                        Id = c.Id,
                        Text = c.Text
                    }).ToList()
                }).ToList()
            });
        }

        [HttpPost("{examId}/submit")]
        public async Task<ActionResult<ExamResultDto>> SubmitExam(int examId, SubmitExamDto dto)
        {
            try
            {
                var result = await _gradingService.SubmitExamAsync(StudentId, examId, dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{examId}/result")]
        public async Task<ActionResult<ExamResultDto>> GetMyResult(int examId)
        {
            var assignment = await _context.ExamStudents
                .Include(es => es.Exam)
                .Include(es => es.Student)
                .FirstOrDefaultAsync(es => es.ExamId == examId && es.StudentId == StudentId && !es.IsDeleted);

            if (assignment == null) return NotFound();

            if (!assignment.IsSubmitted) return BadRequest("Exam not yet submitted.");

            return Ok(new ExamResultDto
            {
                ExamId = examId,
                ExamName = assignment.Exam!.Name,
                StudentId = StudentId,
                StudentName = assignment.Student!.Name,
                CorrectAnswers = assignment.CorrectAnswers ?? 0,
                TotalQuestions = assignment.TotalQuestions ?? 0,
                Score = assignment.Score ?? 0,
                SubmittedAt = assignment.SubmittedAt
            });
        }

        [HttpGet("courses")]
        public async Task<IActionResult> GetMyCourses()
        {
            var courses = await _context.CourseStudents
                .Where(cs => cs.StudentId == StudentId && !cs.IsDeleted)
                .Include(cs => cs.Course)
                .Select(cs => new
                {
                    cs.CourseId,
                    cs.Course!.Name,
                    cs.Course.Description,
                    cs.Course.Hours
                })
                .ToListAsync();

            return Ok(courses);
        }
    }
}
