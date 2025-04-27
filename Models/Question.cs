namespace ExaminationSystem.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Header { get; set; }
        public string Description { get; set; }
        
        public bool Deleted { get; set; }
        public ICollection<Choice> Choices { get; set; }
    }
}
