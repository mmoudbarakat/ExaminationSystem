namespace ExaminationSystem.Models
{
    public class Student : BaseModel
    {
        public ICollection<Exam> Exams { get; set; }
        public ICollection<StudentAnswer> StudentAnswers { get; set; }
    }
}
