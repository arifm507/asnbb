using System.Drawing;

namespace AllamaShibliQuiz.Models.ViewModels
{
    public class TeamViewModel
    {
        public int Id { get; set; }
        public string Role { get; set; }
        public string FullName { get; set; }
        public string FatherName { get; set; }
        public string MobileNumber { get; set; }
        public string AadharNumber { get; set; }
        public string EmailId { get; set; }
        public string PermanentAddress { get; set; }
        public string WhatsappNumber { get; set; }
        public string HigherQualification { get; set; }
        public string AdditionalQualification { get; set; }
        public string OtherSkills { get; set; }
        public string TeachingExperience { get; set; }
        public string CurrentSchoolName { get; set; }
        public DateTime JoiningDate { get; set; }
        public byte[]? Image { get; set; }
        public string? ImageName { get; set; }
        public string? ContentType { get; set; }
        public string? ImageBase64 => Image != null ? Convert.ToBase64String(Image) : null;

    }
}
