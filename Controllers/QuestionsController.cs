using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizMaker.Data;
using QuizMaker.Models;

namespace QuizMaker.Controllers;

public class QuestionsController(ApplicationDbContext ctx, ILogger<QuestionsController> logger) : Controller
{
    private readonly ApplicationDbContext _ctx = ctx;
    private readonly ILogger _log = logger;

    public async Task<IActionResult> Create(int quizId)
    {
        var quiz = await _ctx.Quizzes.FindAsync(quizId);
        if (quiz == null)
            return NotFound();

    
        return View(new Question
        {
            QuizId = quizId,
            Points = 1,
            Options = new List<Option> { new(), new() }
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Question question)
    {
        ModelState.Clear();
        _log.LogInformation(
            "POST /Questions/Create: QuizId={QuizId}, Text='{Text}', Points={Points}, Options={Count}",
            question.QuizId, question.Text, question.Points, question.Options?.Count ?? 0
        );

        // Ensure we always have a list
        question.Options ??= new List<Option>();

        // Clean options (remove empty, trim text)
        var cleaned = new List<Option>();
        foreach (var o in question.Options)
        {
            var text = (o.Text ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(text))
            {
                cleaned.Add(new Option
                {
                    Text = text,
                    IsCorrect = o.IsCorrect
                });
            }
        }

        question.Options = cleaned;

        // Validate
        if (question.Options.Count < 2)
            ModelState.AddModelError(string.Empty, "Provide at least two options.");

        if (!question.Options.Any(o => o.IsCorrect))
            ModelState.AddModelError(string.Empty, "Mark at least one option as correct.");

        if (!ModelState.IsValid)
        {
            // Redisplay form with error messages
            return View(question);
        }

        // Link options back to question for EF relationship
        foreach (var o in question.Options)
            o.Question = question;

        try
        {
            await _ctx.Questions.AddAsync(question);
            await _ctx.SaveChangesAsync();

            _log.LogInformation("Question saved successfully with Id={QuestionId}", question.Id);
            return RedirectToAction("Details", "Quizzes", new { id = question.QuizId });
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Error while saving question");
            ModelState.AddModelError(string.Empty, "Unexpected error while creating the question.");
            return View(question);
        }
    }

    
    public async Task<IActionResult> Edit(int id)
    {
        var q = await _ctx.Questions
            .Include(x => x.Options)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (q == null)
            return NotFound();

        return View(q);
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Question question)
    {
        if (id != question.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(question);

        var existing = await _ctx.Questions
            .Include(x => x.Options)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (existing == null)
            return NotFound();

        existing.Text = question.Text;
        existing.Points = question.Points;

        // Sync options
        _ctx.Options.RemoveRange(existing.Options.Where(o => !question.Options.Any(n => n.Id == o.Id)));

        foreach (var opt in question.Options)
        {
            var e = existing.Options.FirstOrDefault(o => o.Id == opt.Id);
            if (e is null)
            {
                existing.Options.Add(new Option
                {
                    Text = opt.Text,
                    IsCorrect = opt.IsCorrect
                });
            }
            else
            {
                e.Text = opt.Text;
                e.IsCorrect = opt.IsCorrect;
            }
        }

        try
        {
            await _ctx.SaveChangesAsync();
            return RedirectToAction("Details", "Quizzes", new { id = existing.QuizId });
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Error while editing question {QuestionId}", id);
            ModelState.AddModelError(string.Empty, "Unexpected error while updating the question.");
            return View(question);
        }
    }

    
    public async Task<IActionResult> Delete(int id)
    {
        var q = await _ctx.Questions
            .Include(x => x.Options)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (q == null)
            return NotFound();

        return View(q);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var q = await _ctx.Questions.FindAsync(id);
        if (q == null)
            return NotFound();

        var quizId = q.QuizId;
        _ctx.Questions.Remove(q);
        await _ctx.SaveChangesAsync();

        return RedirectToAction("Details", "Quizzes", new { id = quizId });
    }
}
