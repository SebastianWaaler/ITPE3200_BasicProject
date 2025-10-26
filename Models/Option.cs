using System.ComponentModel.DataAnnotations;

namespace QuizMaker.Models;

public class Option
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Text { get; set; } = "";

    public bool IsCorrect { get; set; }

    public int QuestionId { get; set; }
    public Question Question { get; set; } = null!;
}