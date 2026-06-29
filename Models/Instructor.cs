namespace ExaminationSystem.Models
{
    public class Instructor : BaseModel
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public ICollection<Course> Courses { get; set; } = new List<Course>();
        public ICollection<Exam> Exams { get; set; } = new List<Exam>();
        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
