using AuthService.Application.Models.Dtos;
using AuthService.Persistence.Identity;

namespace AuthService.Infrastructure.Extensions;

public static class ConvertExtension
{
    public static UserDto ToUserDto(this ApplicationUser user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            IsEmailVerified = user.EmailConfirmed
        };
    }
}