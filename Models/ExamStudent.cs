namespace ExaminationSystem.Models
{
    public class ExamStudent : BaseModel
    {
        public int ExamId { get; set; }
        public int StudentId { get; set; }
        public Exam Exam { get; set; }
        public Student Student { get; set; }
    }
}
