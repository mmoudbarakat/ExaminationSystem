namespace ExaminationSystem.Models
{
    public class Choice : BaseModel
    {
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public int QuestionId { get; set; }

        public Question Question { get; set; } = null!;
        public ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
    }
}
