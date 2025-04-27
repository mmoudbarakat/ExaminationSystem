using ExaminationSystem.Models;

namespace ExaminationSystem.Dtos
{
    public class QuestionDto
    {
        public string Header { get; set; }
        public ICollection<Choice> Choices { get; set; }
    }
}
