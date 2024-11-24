using EgycastApi.Auth.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Mvc.SignInResult;

namespace EgycastApi.Auth;


[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    
    private readonly IHttpContextAccessor _httpContextAccessor;


    public AuthController(AuthService authService, IHttpContextAccessor httpContextAccessor)
    {
        _authService = authService;
        _httpContextAccessor = httpContextAccessor;
    }

    
    [HttpPost("register")]
    public async Task<IResult> Register([FromBody] UserRegisterDto userRegisterDto)
    {
       var result =  await _authService.Register(userRegisterDto);
       if (result.Succeeded) return Results.Ok("Registered Successfully");
       var problemDetails =  new ProblemDetails { 
            Status = StatusCodes.Status400BadRequest,
            Extensions = new Dictionary<string, object?>
            {
                {"error", result.Errors}
            }
       };
       return Results.BadRequest(problemDetails);
    }
    
    
    [HttpPost("signin")]
    public async Task<IActionResult> SignIn([FromBody] UserLoginDto userLoginDto)
    {
        var response = await _authService.SignIn(userLoginDto);
        _httpContextAccessor.HttpContext!.Response.Cookies.Append("accessToken", response.AccessToken,
            new CookieOptions
            {
                Path = "/",
                MaxAge = TimeSpan.FromDays(1),
                HttpOnly = true
            });
        
        return Ok(response);
    }
    
    
    [HttpDelete("logout")]
    public async Task<IResult> Logout()
    {
        await _authService.Logout();
        _httpContextAccessor.HttpContext!.Response.Cookies.Delete("accessToken");
        return Results.NoContent();
    }
}