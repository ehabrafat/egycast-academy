using System.Security.Claims;
using EgycastApi.Auth.Dtos;
using Microsoft.AspNetCore.Identity;

namespace EgycastApi.Auth;

public interface IAuthService
{
    string GenerateJwtToken(List<Claim> claims);
}