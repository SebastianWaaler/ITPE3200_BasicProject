namespace QuizMaker.Models;

public class Answer
{
    public int Id { get; set; }

    public int SubmissionId { get; set; }
    public Submission Submission { get; set; } = null!;

    public int QuestionId { get; set; }
    public Question Question { get; set; } = null!;

    // store selected Option.Id(s); MVP = single choice
    public int? SelectedOptionId { get; set; }
}
