using System;

namespace ResumeScreeningAPI.Models;

public class JobApplication
{
    public int Id { get; set; }
        public string ApplicantId { get; set; }  
        public string JobTitle { get; set; }
        public string CompanyName { get; set; }
        public string ResumePath { get; set; }  // Stores path of uploaded PDF file
        public string Status { get; set; } = "Pending";
        public DateTime AppliedDate { get; set; } = DateTime.UtcNow;

        public ApplicationUser Applicant { get; set; }
}
