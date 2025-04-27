namespace ExaminationSystem.Models
{
    public class Choice
    {
        public int Id { get; set; }
        public string text { get; set; }
        public int QuestionId{ get; set; }
        public bool Deleted { get; set; }
        public ICollection <Question> Questions { get; set; }


    }
}
