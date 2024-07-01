using System.ComponentModel.DataAnnotations;

namespace AllamaShibliQuiz.Models
{
    public class Team
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Role { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string FatherName { get; set; }
        [Required]
        public string MobileNumber { get; set; }
        [Required]
        public string AadharNumber { get; set; }
        [Required]
        public string EmailId { get; set; }
        public string? PermanentAddress { get; set; }
        public string? WhatsappNumber { get; set; }
        public string? HigherQualification { get; set; }
        public string? AdditionalQualification { get; set; }
        public string? OtherSkills { get; set; }
        public string? TeachingExperience { get; set; }
        public string? CurrentSchoolName { get; set; }
        [Required]
        public DateTime JoiningDate { get; set; }
        public byte[]? Image { get; set; }
        public string? ImageName { get; set; }
        public string? ContentType { get; set; }
    }
}
