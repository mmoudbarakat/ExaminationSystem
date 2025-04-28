using ExaminationSystem.Models.Enums;

namespace ExaminationSystem.Models
{
    public class Exam : BaseModel
    {
        public TimeSpan Duration { get; set; }
        public ExamType ExamType { get; set; }
        public Instructor Instructor   { get; set; }
        public ICollection<Question> Questions { get; set; }
        public ICollection<Student> Students    { get; set; }
        //public int DurationInMinutes { get; set; }
        public ICollection<StudentAnswer> StudentAnswers { get; set; }
    }
}
