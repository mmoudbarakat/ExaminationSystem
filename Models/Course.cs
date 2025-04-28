using System.ComponentModel.DataAnnotations.Schema;

namespace ExaminationSystem.Models
{
    public class Course : BaseModel
    {
        public string Description { get; set; }
        public int Hours { get; set; }
        [InverseProperty("PrerequesitCourse")]
        public List<Prerequesit> Prerequesits { get; set; }
        [InverseProperty("MainCourse")]
        public List <Prerequesit> MainCourse { get; set; }
        public ICollection<Exam> Exams { get; set; }
        public ICollection<Question> Questions { get; set; }
        public ICollection<StudentAnswer> StudentAnswers { get; set; }
    }
}
