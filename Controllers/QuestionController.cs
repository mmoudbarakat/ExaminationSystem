using System.Security.Claims;
using ExaminationSystem.Data;
using ExaminationSystem.Dtos;
using ExaminationSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Instructor")]
    public class QuestionController : ControllerBase
    {
        private readonly Context _context;

        public QuestionController(Context context)
        {
            _context = context;
        }

        private int InstructorId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<ActionResult<List<QuestionDto>>> GetMyQuestions([FromQuery] int? courseId)
        {
            var query = _context.Questions
                .Where(q => q.InstructorId == InstructorId && !q.IsDeleted)
                .Include(q => q.Choices.Where(c => !c.IsDeleted))
                .AsQueryable();

            if (courseId.HasValue)
                query = query.Where(q => q.CourseId == courseId.Value);

            var questions = await query.ToListAsync();
            return Ok(questions.Select(MapQuestionDto).ToList());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionDto>> GetById(int id)
        {
            var question = await _context.Questions
                .Include(q => q.Choices.Where(c => !c.IsDeleted))
                .FirstOrDefaultAsync(q => q.Id == id && q.InstructorId == InstructorId && !q.IsDeleted);

            if (question == null) return NotFound();
            return Ok(MapQuestionDto(question));
        }

        [HttpPost]
        public async Task<ActionResult<QuestionDto>> Create(CreateQuestionDto dto)
        {
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == dto.CourseId && c.InstructorId == InstructorId && !c.IsDeleted);

            if (course == null) return BadRequest("Course not found or not owned by you.");

            if (dto.Choices.Count < 2)
                return BadRequest("A question must have at least 2 choices.");

            if (!dto.Choices.Any(c => c.IsCorrect))
                return BadRequest("At least one choice must be marked as correct.");

            var question = new Question
            {
                Header = dto.Header,
                QuestionLevel = dto.QuestionLevel,
                CourseId = dto.CourseId,
                InstructorId = InstructorId,
                Name = dto.Header,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = InstructorId,
                Choices = dto.Choices.Select(c => new Choice
                {
                    Text = c.Text,
                    IsCorrect = c.IsCorrect,
                    Name = c.Text,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = InstructorId
                }).ToList()
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = question.Id }, MapQuestionDto(question));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateQuestionDto dto)
        {
            var question = await _context.Questions
                .FirstOrDefaultAsync(q => q.Id == id && q.InstructorId == InstructorId && !q.IsDeleted);

            if (question == null) return NotFound();

            question.Header = dto.Header;
            question.Name = dto.Header;
            question.QuestionLevel = dto.QuestionLevel;
            question.UpdatedBy = InstructorId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var question = await _context.Questions
                .FirstOrDefaultAsync(q => q.Id == id && q.InstructorId == InstructorId && !q.IsDeleted);

            if (question == null) return NotFound();

            question.IsDeleted = true;
            question.DeletedBy = InstructorId;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private static QuestionDto MapQuestionDto(Question q) => new()
        {
            Id = q.Id,
            Header = q.Header,
            QuestionLevel = q.QuestionLevel,
            CourseId = q.CourseId,
            Choices = q.Choices?
                .Where(c => !c.IsDeleted)
                .Select(c => new ChoiceDto { Id = c.Id, Text = c.Text, IsCorrect = c.IsCorrect })
                .ToList() ?? new List<ChoiceDto>()
        };
    }
}
