using ExaminationSystem.Models.Enums;

namespace ExaminationSystem.Models
{
    public class Question : BaseModel
    {

        public string Header { get; set; }

        public QuestionLevel QuestionLevel { get; set; }

        public ICollection<Choice> Choices { get; set; }
        public Course Course { get; set; }
        public ICollection<Exam> Exams { get; set; }
        public int CourseId { get; set; }
        public Instructor Instructor { get; set; }
        public ICollection<StudentAnswer> StudentAnswers { get; set; }
    }
}
