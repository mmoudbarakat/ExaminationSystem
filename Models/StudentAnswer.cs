namespace ExaminationSystem.Models
{
    public class StudentAnswer : BaseModel
    {
        public int StudentId { get; set; }
        public int ExamId { get; set; }
        public int QuestionId { get; set; }
        public int ChoiceId { get; set; }
        public int CourseId { get; set; }
        public bool IsCorrect { get; set; }

        public Student Student { get; set; } = null!;
        public Exam Exam { get; set; } = null!;
        public Question Question { get; set; } = null!;
        public Choice Choice { get; set; } = null!;
        public Course Course { get; set; } = null!;
    }
}
