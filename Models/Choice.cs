namespace ExaminationSystem.Models
{
    public class Choice : BaseModel
    {
        public string text { get; set; }
        public bool IsCorrect { get; set; }
        public int QuestionId{ get; set; }
        public ICollection <Question> Questions { get; set; }
        public ICollection<StudentAnswer> StudentAnswers { get; set; }

    }
}
