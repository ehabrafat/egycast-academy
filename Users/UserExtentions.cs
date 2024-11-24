using EgycastApi.Users.Dtos;

namespace EgycastApi.Users;

public static class UserExtentions
{
    public static UserResDto ToResDto(this AppUser user)
    {
        return new UserResDto { Id = user.Id, Username = user.UserName!, Email = user.Email!};
    }
}