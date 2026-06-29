namespace ExaminationSystem.Models
{
    public class CourseStudent : BaseModel
    {
        public int CourseId { get; set; }
        public int StudentId { get; set; }

        public Course Course { get; set; } = null!;
        public Student Student { get; set; } = null!;
    }
}
