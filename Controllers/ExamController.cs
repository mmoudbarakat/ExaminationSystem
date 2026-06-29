using System.Security.Claims;
using ExaminationSystem.Data;
using ExaminationSystem.Dtos;
using ExaminationSystem.Models;
using ExaminationSystem.Models.Enums;
using ExaminationSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Instructor")]
    public class ExamController : ControllerBase
    {
        private readonly Context _context;
        private readonly ExamAssignmentService _assignmentService;

        public ExamController(Context context, ExamAssignmentService assignmentService)
        {
            _context = context;
            _assignmentService = assignmentService;
        }

        private int InstructorId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<ActionResult<List<ExamDto>>> GetMyExams()
        {
            var exams = await _context.Exams
                .Where(e => e.InstructorId == InstructorId && !e.IsDeleted)
                .Include(e => e.ExamQuestions)
                .ToListAsync();

            return Ok(exams.Select(MapExamDto).ToList());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExamDto>> GetById(int id)
        {
            var exam = await _context.Exams
                .Include(e => e.ExamQuestions)
                .FirstOrDefaultAsync(e => e.Id == id && e.InstructorId == InstructorId && !e.IsDeleted);

            if (exam == null) return NotFound();
            return Ok(MapExamDto(exam));
        }

        [HttpPost]
        public async Task<ActionResult<ExamDto>> Create(CreateExamDto dto)
        {
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == dto.CourseId && c.InstructorId == InstructorId && !c.IsDeleted);

            if (course == null) return BadRequest("Course not found or not owned by you.");

            var exam = new Exam
            {
                Name = dto.Name,
                Description = dto.Description,
                ExamType = dto.ExamType,
                CourseId = dto.CourseId,
                InstructorId = InstructorId,
                QuestionCount = dto.QuestionCount,
                IsAutoAssign = dto.IsAutoAssign,
                Duration = TimeSpan.FromMinutes(dto.DurationMinutes),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = InstructorId
            };

            _context.Exams.Add(exam);
            await _context.SaveChangesAsync();

            try
            {
                await _assignmentService.AssignQuestionsAsync(exam, dto.QuestionIds);
            }
            catch (InvalidOperationException ex)
            {
                _context.Exams.Remove(exam);
                await _context.SaveChangesAsync();
                return BadRequest(ex.Message);
            }

            await _context.Entry(exam).Collection(e => e.ExamQuestions).LoadAsync();
            return CreatedAtAction(nameof(GetById), new { id = exam.Id }, MapExamDto(exam));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateExamDto dto)
        {
            var exam = await _context.Exams
                .Include(e => e.ExamQuestions)
                .FirstOrDefaultAsync(e => e.Id == id && e.InstructorId == InstructorId && !e.IsDeleted);

            if (exam == null) return NotFound();

            exam.Name = dto.Name;
            exam.Description = dto.Description;
            exam.ExamType = dto.ExamType;
            exam.QuestionCount = dto.QuestionCount;
            exam.IsAutoAssign = dto.IsAutoAssign;
            exam.Duration = TimeSpan.FromMinutes(dto.DurationMinutes);
            exam.UpdatedBy = InstructorId;

            try
            {
                await _assignmentService.AssignQuestionsAsync(exam, dto.QuestionIds);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var exam = await _context.Exams
                .FirstOrDefaultAsync(e => e.Id == id && e.InstructorId == InstructorId && !e.IsDeleted);

            if (exam == null) return NotFound();

            exam.IsDeleted = true;
            exam.DeletedBy = InstructorId;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("{examId}/assign/{studentId}")]
        public async Task<IActionResult> AssignStudent(int examId, int studentId)
        {
            var exam = await _context.Exams
                .FirstOrDefaultAsync(e => e.Id == examId && e.InstructorId == InstructorId && !e.IsDeleted);

            if (exam == null) return NotFound("Exam not found.");

            var enrolled = await _context.CourseStudents
                .AnyAsync(cs => cs.CourseId == exam.CourseId && cs.StudentId == studentId && !cs.IsDeleted);

            if (!enrolled) return BadRequest("Student must be enrolled in the course first.");

            if (exam.ExamType == ExamType.Final)
            {
                var hasFinal = await _context.ExamStudents
                    .AnyAsync(es => es.StudentId == studentId
                        && !es.IsDeleted
                        && es.Exam!.ExamType == ExamType.Final
                        && es.Exam.CourseId == exam.CourseId);

                if (hasFinal)
                    return BadRequest("Student already has a final exam assigned for this course.");
            }

            var exists = await _context.ExamStudents
                .AnyAsync(es => es.ExamId == examId && es.StudentId == studentId && !es.IsDeleted);

            if (exists) return BadRequest("Student already assigned to this exam.");

            _context.ExamStudents.Add(new ExamStudent
            {
                ExamId = examId,
                StudentId = studentId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = InstructorId
            });

            await _context.SaveChangesAsync();
            return Ok(new { message = "Student assigned to exam." });
        }

        [HttpGet("{examId}/results")]
        public async Task<ActionResult<List<ExamResultDto>>> GetExamResults(int examId)
        {
            var exam = await _context.Exams
                .FirstOrDefaultAsync(e => e.Id == examId && e.InstructorId == InstructorId && !e.IsDeleted);

            if (exam == null) return NotFound();

            var results = await _context.ExamStudents
                .Where(es => es.ExamId == examId && es.IsSubmitted && !es.IsDeleted)
                .Include(es => es.Student)
                .Select(es => new ExamResultDto
                {
                    ExamId = examId,
                    ExamName = exam.Name,
                    StudentId = es.StudentId,
                    StudentName = es.Student!.Name,
                    CorrectAnswers = es.CorrectAnswers ?? 0,
                    TotalQuestions = es.TotalQuestions ?? 0,
                    Score = es.Score ?? 0,
                    SubmittedAt = es.SubmittedAt
                })
                .ToListAsync();

            return Ok(results);
        }

        private static ExamDto MapExamDto(Exam exam) => new()
        {
            Id = exam.Id,
            Name = exam.Name,
            Description = exam.Description,
            ExamType = exam.ExamType,
            CourseId = exam.CourseId,
            QuestionCount = exam.QuestionCount,
            IsAutoAssign = exam.IsAutoAssign,
            DurationMinutes = exam.Duration.TotalMinutes,
            QuestionIds = exam.ExamQuestions?.Select(eq => eq.QuestionId).ToList() ?? new List<int>()
        };
    }
}
