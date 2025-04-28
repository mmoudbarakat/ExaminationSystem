namespace ExaminationSystem.Models
{
    public class ExamQuestion : BaseModel
    {
        public int ExamId { get; set; }
        public int QuestionId { get; set; }
        public  Exam Exam { get; set; }
        public  Question Question { get; set; }

    }
}
