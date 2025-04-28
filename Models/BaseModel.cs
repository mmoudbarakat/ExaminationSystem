namespace ExaminationSystem.Models
{
    public class BaseModel
    {
        public int Id{ get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public int DeletedBy { get; set; }
        public bool IsNew { get; set; }
        

    }
}
