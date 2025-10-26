namespace QuizMaker.Models
{
    public class QuizSubmissionViewModel
    {
        public int QuizId { get; set; }
        public string ParticipantName { get; set; } = string.Empty;
        public Dictionary<int, int?> UserAnswers { get; set; } = new();
        public Quiz? Quiz { get; set; }
    }
}
