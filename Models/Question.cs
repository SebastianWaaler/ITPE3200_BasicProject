using System.ComponentModel.DataAnnotations;

namespace QuizMaker.Models;

public class Question
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Text { get; set; } = "";

    [Range(0, 100)]
    public int Points { get; set; } = 1;

    public int QuizId { get; set; }
    public Quiz Quiz { get; set; } = null!;

    public List<Option> Options { get; set; } = new();
    
}