using ExaminationSystem.Data;
using ExaminationSystem.Dtos;
using ExaminationSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Controllers
{
    public class QuestionController : Controller
    {
        private readonly Context _context;
        public QuestionController(Context context)
        {
            context = _context;
        }



        [HttpPost]
        public async Task<IActionResult> Add([FromBody] QuestionDto questionDto)
        {
            var question = new Question
            {
                Header = questionDto.Header,
                Choices = questionDto.Choices.Select(ChoiceDto => new Choice
                {
                    text = ChoiceDto.text
                }).ToList(),
            };


            await _context.Questions.AddAsync(question);
            await _context.SaveChangesAsync();
            return Ok(question);
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, [FromBody] QuestionDto questionDto)
        {
            var question = await _context.Questions.FirstOrDefaultAsync(x => x.Id == id);
            if (question is null)
                return NotFound("Question not found!");
            question.Header = questionDto.Header;


            await _context.SaveChangesAsync();

            return Ok(question);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var question = await _context.Questions.FirstOrDefaultAsync(q => q.Id == id);
            var choice = await _context.Choices.FirstOrDefaultAsync(c => c.Id == id);

            if (question is null)
                return NotFound("Could not delete question!");
            question.Deleted = true;
            choice.Deleted = true;

            await _context.SaveChangesAsync();
            

            return Ok("Question Deleted.");
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}
