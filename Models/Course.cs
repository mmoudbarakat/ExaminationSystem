namespace ExaminationSystem.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int Hour { get; set; }
        public bool Deleted { get; set; }
    }
}
