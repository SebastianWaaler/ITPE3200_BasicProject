using System.ComponentModel.DataAnnotations;

namespace QuizMaker.Models;

public class Submission
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string ParticipantName { get; set; } = "";

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public int QuizId { get; set; }
    public Quiz Quiz { get; set; } = null!;

    public List<Answer> Answers { get; set; } = new();

    public int Score { get; set; }
    public int MaxScore { get; set; }
}
