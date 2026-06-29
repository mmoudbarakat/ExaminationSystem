using ExaminationSystem.Models.Enums;

namespace ExaminationSystem.Models
{
    public class Question : BaseModel
    {
        public string Header { get; set; } = string.Empty;
        public QuestionLevel QuestionLevel { get; set; }
        public int CourseId { get; set; }
        public int InstructorId { get; set; }

        public Course Course { get; set; } = null!;
        public Instructor Instructor { get; set; } = null!;
        public ICollection<Choice> Choices { get; set; } = new List<Choice>();
        public ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();
        public ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
    }
}
