namespace N10.Services.Interfaces;

public interface ICurrentUserService
{
    Task<UserDto?> GetUserAsync();
}