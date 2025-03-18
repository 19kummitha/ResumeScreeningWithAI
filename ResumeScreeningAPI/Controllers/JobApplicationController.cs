using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeScreeningAPI.Data;
using ResumeScreeningAPI.Models;

namespace ResumeScreeningAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JobApplicationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string _resumeUploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        public JobApplicationController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

            if (!Directory.Exists(_resumeUploadPath))
            {
                Directory.CreateDirectory(_resumeUploadPath); // Ensure the upload folder exists
            }
        }
        [HttpPost]
        public async Task<IActionResult> ApplyForJob(
            [FromForm] string jobTitle,
            [FromForm] string companyName,
            [FromForm] IFormFile resumeFile)
        {
            if (resumeFile == null || resumeFile.Length == 0)
                return BadRequest("Resume file is required.");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized("User not found");

            // Generate unique file name
            string fileName = $"{Guid.NewGuid()}_{resumeFile.FileName}";
            string filePath = Path.Combine(_resumeUploadPath, fileName);

            // Save file to server
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await resumeFile.CopyToAsync(stream);
            }

            // Create job application entry
            var jobApplication = new JobApplication
            {
                ApplicantId = userId,
                JobTitle = jobTitle,
                CompanyName = companyName,
                ResumePath = filePath
            };

            _context.JobApplications.Add(jobApplication);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Job application submitted successfully", resumePath = filePath });
        }

        // Get user's job applications
        [HttpGet("my")]
        public async Task<IActionResult> GetMyApplications()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized("User not found");

            var applications = await _context.JobApplications
                .Where(j => j.ApplicantId == userId)
                .ToListAsync();

            return Ok(applications);
        }

        // Get all job applications (Admin/Recruiter)
        [HttpGet]
        [Authorize(Roles = "Admin,Recruiter")]
        public async Task<IActionResult> GetAllApplications()
        {
            var applications = await _context.JobApplications.ToListAsync();
            return Ok(applications);
        }

        // Update application status (Admin/Recruiter)
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,Recruiter")]
        public async Task<IActionResult> UpdateApplicationStatus(int id, [FromBody] string status)
        {
            var application = await _context.JobApplications.FindAsync(id);
            if (application == null) return NotFound("Job application not found");

            application.Status = status;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Application status updated successfully" });
        }

        // Download resume file
        [HttpGet("{id}/resume")]
        public async Task<IActionResult> DownloadResume(int id)
        {
            var application = await _context.JobApplications.FindAsync(id);
            if (application == null || string.IsNullOrEmpty(application.ResumePath))
                return NotFound("Resume not found");

            var filePath = application.ResumePath;
            var fileName = Path.GetFileName(filePath);
            var mimeType = "application/pdf";

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, mimeType, fileName);
        }
    }
}
