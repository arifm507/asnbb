using System.ComponentModel.DataAnnotations;

namespace AllamaShibliQuiz.Models
{
    public class School
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Address { get; set; }
        public int CentreCode { get; set; }
        public bool IsExamCentre { get; set; }
        public bool IsExternalExamCentre { get; set; }
        public int? Rank { get; set; } 
        public bool IsActive { get; set; }
        [Required]
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdateDate { get; set; }
    }
}
