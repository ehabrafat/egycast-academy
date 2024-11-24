using EgycastApi.Auth;
using EgycastApi.Users.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EgycastApi.Users;

[ApiController]
[Route("users")]
[Authorize(Policy = "authenticated")]
public class UserController : ControllerBase
{
    private readonly AuthService _authService;

    public UserController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var user = await _authService.GetCurrentUser();
        return Ok(new StorageUserRes
        {
            
            Id = user.Id,
            Username = user.UserName,
            ImgUrl = user.ImgUrl
        });
    }
}