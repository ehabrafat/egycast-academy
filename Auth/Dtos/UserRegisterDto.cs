using System.ComponentModel.DataAnnotations;

namespace EgycastApi.Auth.Dtos;

public class UserRegisterDto
{
    [Required, Length(4, 20, ErrorMessage = "Username must be between 4-20 characters")]
    public string Username { get; set; }
    
    [Required, EmailAddress(ErrorMessage = "Email must be valid")]
    public string Email { get; set; }
    
    [Required, MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; }
}