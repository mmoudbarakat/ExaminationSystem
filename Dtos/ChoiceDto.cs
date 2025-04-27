using ExaminationSystem.Models;

namespace ExaminationSystem.Dtos
{
    public class ChoiceDto
    {
        public string text { get; set; }
        public ICollection<Question> questions{ get; set; }
    }
}
