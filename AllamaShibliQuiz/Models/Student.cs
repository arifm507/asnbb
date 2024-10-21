using System.ComponentModel.DataAnnotations;

namespace AllamaShibliQuiz.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string FatherName { get; set; }
        [Required]
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [Required]
        public int Class { get; set; }
        public int? SchoolId { get; set; }
        public string? Subject { get; set; }
        [Required]
        public string SchoolName { get; set; }
        [Required]
        public int ExamCentreId { get; set; }
        [Required]
        public string Address { get; set; }
        public string? RollNumber { get; set; }
        [Required]
        public string MobileNumber { get; set; }
        [Required]
        public string AadharNumber { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int Status { get; set; }

    }
}
