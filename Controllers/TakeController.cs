using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizMaker.Data;
using QuizMaker.Models;

namespace QuizMaker.Controllers;

public class TakeController(ApplicationDbContext ctx, ILogger<TakeController> logger) : Controller
{
    private readonly ApplicationDbContext _ctx = ctx;
    private readonly ILogger _log = logger;

    public async Task<IActionResult> Start(int id)
{
    var quiz = await _ctx.Quizzes
        .Include(q => q.Questions)
        .ThenInclude(q => q.Options)
        .FirstOrDefaultAsync(q => q.Id == id);

    if (quiz == null) return NotFound();

    // Wrap the quiz inside view model
    var model = new QuizSubmissionViewModel
    {
        QuizId = quiz.Id,
        Quiz = quiz
    };

    return View(model);
}


[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Submit(QuizSubmissionViewModel model)
{
    var quiz = await _ctx.Quizzes
        .Include(q => q.Questions)
        .ThenInclude(q => q.Options)
        .FirstOrDefaultAsync(q => q.Id == model.QuizId);

    if (quiz == null)
        return NotFound();

    if (string.IsNullOrWhiteSpace(model.ParticipantName))
    {
        TempData["Error"] = "Please enter your name.";
        return RedirectToAction(nameof(Start), new { id = model.QuizId });
    }

    var submission = new Submission
    {
        ParticipantName = model.ParticipantName.Trim(),
        QuizId = quiz.Id
    };

    int score = 0, max = 0;

    foreach (var q in quiz.Questions)
    {
        max += q.Points;
        model.UserAnswers.TryGetValue(q.Id, out var selected);

        submission.Answers.Add(new Answer
        {
            QuestionId = q.Id,
            SelectedOptionId = selected
        });

        if (selected.HasValue &&
            q.Options.FirstOrDefault(o => o.Id == selected.Value)?.IsCorrect == true)
        {
            score += q.Points;
        }
    }

    submission.Score = score;
    submission.MaxScore = max;

    try
    {
        await _ctx.Submissions.AddAsync(submission);
        await _ctx.SaveChangesAsync();
    }
    catch (Exception ex)
    {
        _log.LogError(ex, "Submit quiz failed");
        TempData["Error"] = "Unexpected error submitting your quiz. Please try again.";
        return RedirectToAction(nameof(Start), new { id = model.QuizId });
    }

    return RedirectToAction(nameof(Result), new { id = submission.Id });
}


    public async Task<IActionResult> Result(int id)
    {
        var s = await _ctx.Submissions
            .Include(x => x.Quiz)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (s == null) return NotFound();
        return View(s);
    }
}
