using Microsoft.AspNetCore.Mvc;

namespace EgycastApi.Auth.Dtos;

public class UserSignInResponse
{
    public string UserId { get; set; }
    public string Username { get; set; }
    public string ImgUrl { get; set; }
    public string AccessToken { get; set; }
    
    public string RefreshToken { get; set; }
}