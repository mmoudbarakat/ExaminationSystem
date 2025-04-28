namespace ExaminationSystem.Models
{
    public class StudentAnswer : BaseModel
    {
        public int StudentId { get; set; }
        public int ExamId { get; set; }
        public int QuestionId { get; set; }
        public int ChoiceId { get; set; }
        public int CourseId { get; set; }


        public Student Student { get; set; }
        public Exam Exam { get; set; }
        public Question Question { get; set; }
        public Choice Choice { get; set; }
        public Course Course { get; set; }

    }
}
