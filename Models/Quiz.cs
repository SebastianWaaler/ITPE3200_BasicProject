using System.ComponentModel.DataAnnotations;

namespace QuizMaker.Models;

public class Quiz
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Title { get; set; } = "";

    [StringLength(500)]
    public string? Description { get; set; } = ""; ///string? makes it optional by allowing null values.

    public List<Question> Questions { get; set; } = new();
}