using System;
using Microsoft.EntityFrameworkCore;
using ResumeScreeningAPI.Models;

namespace ResumeScreeningAPI.Data;

public class ResumeDbContext(DbContextOptions<ResumeDbContext> options) : DbContext(options)
{
  public DbSet<Resume> Resumes { get; set; }
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Resume>().HasIndex(r => r.Email).IsUnique();
  }
}
