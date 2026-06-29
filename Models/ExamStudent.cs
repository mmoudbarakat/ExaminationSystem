namespace ExaminationSystem.Models
{
    public class ExamStudent : BaseModel
    {
        public int ExamId { get; set; }
        public int StudentId { get; set; }
        public bool IsSubmitted { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public int? CorrectAnswers { get; set; }
        public int? TotalQuestions { get; set; }
        public double? Score { get; set; }

        public Exam Exam { get; set; } = null!;
        public Student Student { get; set; } = null!;
    }
}
