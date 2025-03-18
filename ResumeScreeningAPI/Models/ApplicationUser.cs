using System;
using Microsoft.AspNetCore.Identity;

namespace ResumeScreeningAPI.Models;

public class ApplicationUser:IdentityUser
{
    public string FullName { get; set; }
}
