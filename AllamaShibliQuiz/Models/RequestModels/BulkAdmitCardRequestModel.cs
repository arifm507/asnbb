using System.ComponentModel.DataAnnotations;

namespace AllamaShibliQuiz.Models.RequestModels
{
    public class BulkAdmitCardRequestModel
    {
        [Required]
        public int ExamCentreId { get; set; }

        [Required]
        [Range(6, 12)]
        public int ClassNumber { get; set; }
    }
}
