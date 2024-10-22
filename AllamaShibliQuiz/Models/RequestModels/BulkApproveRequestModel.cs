using System.ComponentModel.DataAnnotations;

namespace AllamaShibliQuiz.Models.RequestModels
{
    public class BulkApproveRequestModel
    {
        [Required]
        public int ExamCentreId { get; set; }
        [Required]
        public int ClassNumber { get; set; }
    }
}
