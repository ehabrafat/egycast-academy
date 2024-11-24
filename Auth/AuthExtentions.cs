using EgycastApi.Auth.Dtos;

namespace EgycastApi.Auth;

public static class AuthExtentions
{
    public static AppUser ToAppUser(this UserRegisterDto userRegisterDto)
    {
        return new AppUser { UserName = userRegisterDto.Username, Email = userRegisterDto.Email };
    }
  
}