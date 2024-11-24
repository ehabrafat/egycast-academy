using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EgycastApi.Auth.Dtos;
using EgycastApi.Config;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EgycastApi.Auth;

public class AuthService : IAuthService
{
    private readonly JwtConfig _jwtConfig;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ApiDbContext _dbContext;
    
    public AuthService(ApiDbContext dbContext, IOptions<JwtConfig> jwtConfigOptions,  UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _jwtConfig = jwtConfigOptions.Value;
        _userManager = userManager;
        _signInManager = signInManager;
        _dbContext = dbContext;
    }

    public async Task<AppUser?> GetCurrentUser()
    {
        string? userId = GetUserClaim("id");
        if (userId is null) return null;
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
        return user;
    }
    
    public string? GetUserClaim(string claim)
    {
        var user =  _signInManager.Context.User;
        return user.Claims.FirstOrDefault(x => x.Type == claim)?.Value;
    }
    public async Task<UserSignInResponse> SignIn(UserLoginDto userLoginDto)
    {
        var user = await _userManager.FindByNameAsync(userLoginDto.Username);
        if(user is null) throw new EgycastException("Invalid Credentials", StatusCodes.Status401Unauthorized);
        var result = await _signInManager.PasswordSignInAsync(user, userLoginDto.Password, false, false);
        if (!result.Succeeded)
        {
            throw new EgycastException("Invalid Credentials", StatusCodes.Status401Unauthorized);
        }
        var response = new UserSignInResponse
        {
            UserId = user.Id,
            Username = user.UserName!,
            ImgUrl = user.ImgUrl!
        };
        
        var claims = new List<Claim>
        {
            new Claim("id", user.Id),
            new Claim("username", user.UserName!),
        };
        response.AccessToken = GenerateJwtToken(claims);
        return response;

    }
    public Task<IdentityResult> Register(UserRegisterDto userRegisterDto)
    {
        AppUser user = userRegisterDto.ToAppUser();
        return _userManager.CreateAsync(user, userRegisterDto.Password);
    }


    public Task Logout()
    {
        return _signInManager.SignOutAsync();
    }
    public string GenerateJwtToken(List<Claim> claims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var subject = new ClaimsIdentity(claims);
        var key = Encoding.UTF8.GetBytes(_jwtConfig.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = subject,
            Expires = DateTime.UtcNow.AddDays(1), // modify when refresh token added
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}