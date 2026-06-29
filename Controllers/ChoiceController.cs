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
    public class ChoiceController : ControllerBase
    {
        private readonly Context _context;

        public ChoiceController(Context context)
        {
            _context = context;
        }

        private int InstructorId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost("{questionId}")]
        public async Task<ActionResult<ChoiceDto>> Add(int questionId, CreateChoiceDto dto)
        {
            var question = await _context.Questions
                .FirstOrDefaultAsync(q => q.Id == questionId && q.InstructorId == InstructorId && !q.IsDeleted);

            if (question == null) return NotFound("Question not found.");

            var choice = new Choice
            {
                Text = dto.Text,
                IsCorrect = dto.IsCorrect,
                QuestionId = questionId,
                Name = dto.Text,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = InstructorId
            };

            _context.Choices.Add(choice);
            await _context.SaveChangesAsync();

            return Ok(new ChoiceDto { Id = choice.Id, Text = choice.Text, IsCorrect = choice.IsCorrect });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateChoiceDto dto)
        {
            var choice = await _context.Choices
                .Include(c => c.Question)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (choice == null || choice.Question!.InstructorId != InstructorId)
                return NotFound();

            choice.Text = dto.Text;
            choice.IsCorrect = dto.IsCorrect;
            choice.Name = dto.Text;
            choice.UpdatedBy = InstructorId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var choice = await _context.Choices
                .Include(c => c.Question)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (choice == null || choice.Question!.InstructorId != InstructorId)
                return NotFound();

            choice.IsDeleted = true;
            choice.DeletedBy = InstructorId;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
