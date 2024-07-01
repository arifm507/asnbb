namespace AllamaShibliQuiz.Models.ViewModels
{
    public class DashboardViewModel
    {
        public List<StudentViewModel> Students { get; set; }
        public int TotalTeamMember { get; set; }
        public int TotalStudentRegistered { get; set; }
        public int TotalStudentApproved { get; set; }
        public int TotalStudentPending { get; set; }
        public int TotalStudentRejected { get; set; }
    }
}
