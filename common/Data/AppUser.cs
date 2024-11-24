using Microsoft.AspNetCore.Identity;

namespace EgycastApi;

public class AppUser : IdentityUser
{
    public string? Bio { get; set; }
    public string? ImgUrl { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}