namespace ExaminationSystem.Models
{
    public class Course : BaseModel
    {
        public int Hours { get; set; }
        public int InstructorId { get; set; }

        public Instructor Instructor { get; set; } = null!;
        public ICollection<CourseStudent> CourseStudents { get; set; } = new List<CourseStudent>();
        public ICollection<Exam> Exams { get; set; } = new List<Exam>();
        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
        public List<Prerequesit> Prerequesits { get; set; } = new List<Prerequesit>();
        public List<Prerequesit> MainCourse { get; set; } = new List<Prerequesit>();
    }
}
