using System;

namespace ResumeScreeningAPI.Models;

public class Resume
{
  public int Id { get; set; }
  public string? CandidateName { get; set; }
  public string? Email { get; set; }
  public string? Skills { get; set; }
  public string? Experience { get; set; }
  public string? Education { get; set; }
  public string? FilePath { get; set; }
  public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
