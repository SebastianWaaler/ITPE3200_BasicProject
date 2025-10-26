using Microsoft.AspNetCore.Mvc;
using QuizMaker.Data.Repositories;
using QuizMaker.Models;

namespace QuizMaker.Controllers;

public class QuizzesController(IQuizRepository repo, ILogger<QuizzesController> logger) : Controller
{
    private readonly IQuizRepository _repo = repo;
    private readonly ILogger _log = logger;


    public async Task<IActionResult> Index()
    {
        var quizzes = await _repo.GetAllAsync();
        return View(quizzes);
    }


    public async Task<IActionResult> Details(int id)
    {
        var quiz = await _repo.GetWithQuestionsAsync(id); 

        if (quiz == null)
            return NotFound();

        return View(quiz);
    }

    // GET: /Quizzes/Create
    public IActionResult Create() => View(new Quiz());

    // POST: /Quizzes/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Quiz quiz)
    {
        if (!ModelState.IsValid)
            return View(quiz);

        try
        {
            await _repo.AddAsync(quiz);
            await _repo.SaveAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to create quiz");
            ModelState.AddModelError("", "Unexpected error while creating the quiz.");
            return View(quiz);
        }
    }

    // GET: /Quizzes/Edit
    public async Task<IActionResult> Edit(int id)
    {
        var quiz = await _repo.GetByIdAsync(id);
        if (quiz == null)
            return NotFound();

        return View(quiz);
    }

    // POST: /Quizzes/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Quiz quiz)
    {
        if (id != quiz.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(quiz);

        try
        {
            await _repo.UpdateAsync(quiz);
            await _repo.SaveAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to edit quiz {QuizId}", id);
            ModelState.AddModelError("", "Unexpected error while updating the quiz.");
            return View(quiz);
        }
    }

    // GET: /Quizzes/Delete
    public async Task<IActionResult> Delete(int id)
    {
        var quiz = await _repo.GetByIdAsync(id);
        if (quiz == null)
            return NotFound();

        return View(quiz);
    }

    // POST: /Quizzes/Delete
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var quiz = await _repo.GetByIdAsync(id);
        if (quiz == null)
            return NotFound();

        try
        {
            await _repo.DeleteAsync(quiz);
            await _repo.SaveAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to delete quiz {QuizId}", id);
            ModelState.AddModelError("", "Unexpected error while deleting the quiz.");
            return View(quiz);
        }
    }
}
