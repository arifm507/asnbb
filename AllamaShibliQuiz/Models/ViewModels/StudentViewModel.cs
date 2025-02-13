﻿namespace AllamaShibliQuiz.Models.ViewModels
{
    public class StudentViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int Class { get; set; }
        public string? Subject { get; set; }
        public int? SchoolId { get; set; }
        public string? SchoolName { get; set; }
        public string? OtherSchoolName { get; set; }
        public int ExamCentreId { get; set; }
        public string? ExamCentreName { get; set; }
        public string? ExamCentreCode { get; set; }
        public string? ExamDate { get; set; }
        public string? RollNumber { get; set; }
        public string Address { get; set; }
        public string MobileNumber { get; set; }
        public string AadharNumber { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int Status { get; set; }
    }
}