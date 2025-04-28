using System.ComponentModel.DataAnnotations.Schema;

namespace ExaminationSystem.Models
{
    public class Prerequesit : BaseModel
    {
        [ForeignKey("MainCourse")]
        public int CourseId { get; set; }
        [ForeignKey("PrerequesitCourse")]
        public int PrerequesitId { get; set; }
        public Course MainCourse { get; set; }
        public Course PrerequesitCourse { get; set; }
        public bool IsMandatory { get; set; } = false;
    }
}
