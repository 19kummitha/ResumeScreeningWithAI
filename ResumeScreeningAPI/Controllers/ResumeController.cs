using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResumeScreeningAPI.Data;
using ResumeScreeningAPI.Models;

namespace ResumeScreeningAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResumeController(ResumeDbContext dbContext) : ControllerBase
    {
        private readonly ResumeDbContext _context = dbContext;
        [HttpPost("upload")]
        public async Task<IActionResult> UploadResume([FromForm] IFormFile file, [FromForm] string candidateName, [FromForm] string email)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Invalid file");

            var filePath = Path.Combine("Resumes", file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var resume = new Resume
            {
                CandidateName = candidateName,
                Email = email,
                FilePath = filePath
            };

            _context.Resumes.Add(resume);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Resume uploaded successfully!" });
        }
    }
}
