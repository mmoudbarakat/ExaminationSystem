using ExaminationSystem.Models.Enums;

namespace ExaminationSystem.Models
{
    public class Exam : BaseModel
    {
        public TimeSpan Duration { get; set; }
        public ExamType ExamType { get; set; }
        public int CourseId { get; set; }
        public int InstructorId { get; set; }
        public int QuestionCount { get; set; }
        public bool IsAutoAssign { get; set; }

        public Course Course { get; set; } = null!;
        public Instructor Instructor { get; set; } = null!;
        public ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();
        public ICollection<ExamStudent> ExamStudents { get; set; } = new List<ExamStudent>();
        public ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
    }
}
