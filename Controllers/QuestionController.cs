using ExaminationSystem.Data;
using ExaminationSystem.Models;
using ExaminationSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design.Internal;

namespace ExaminationSystem.Controllers
{
    public class QuestionController : Controller
    {
        GeneralRepository<Question> _questionRepository;

        public QuestionController()
        {
            _questionRepository = new GeneralRepository<Question>();
        }

        // GET: /Question
        public async Task<IActionResult> Index()
        {
            var questions = await _questionRepository.GetAllAsync();
            return View(questions);
        }

        // GET: /Question/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var question = await _questionRepository.GetByIdAsync(id);
            if (question == null)
            {
                return NotFound();
            }
            return View(question);
        }

        // GET: /Question/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Question/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Question question)
        {
            if (ModelState.IsValid)
            {
                await _questionRepository.AddAsync(question);
                return RedirectToAction(nameof(Index));
            }
            return View(question);
        }

        // GET: /Question/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var question = await _questionRepository.GetByIdAsync(id);
            if (question == null)
            {
                return NotFound();
            }
            return View(question);
        }

        // POST: /Question/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Question question)
        {
            if (id != question.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _questionRepository.UpdateAsync(question);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _questionRepository.ExistsAsync(id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(question);
        }

        // GET: /Question/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var question = await _questionRepository.GetByIdAsync(id);
            if (question == null)
            {
                return NotFound();
            }
            return View(question);
        }

        // POST: /Question/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var question = await _questionRepository.GetByIdAsync(id);
            if (question != null)
            {
                await _questionRepository.DeleteAsync(question);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
