namespace ExaminationSystem.Models
{
    public class Instructor : BaseModel
    {
        public ICollection<Exam> Exams { get; set; }
        public ICollection<Question> Questions { get; set; }
    }
}
