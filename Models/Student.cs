namespace ExaminationSystem.Models
{
    public class Student : BaseModel
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public ICollection<CourseStudent> CourseStudents { get; set; } = new List<CourseStudent>();
        public ICollection<ExamStudent> ExamStudents { get; set; } = new List<ExamStudent>();
        public ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
    }
}
