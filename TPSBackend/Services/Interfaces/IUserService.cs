using TPSBackend.Dtos;
using TPSBackend.Models;

namespace TPSBackend.Services.Interfaces;

public interface IUserService
{
    Task<bool> CreateUserAsync(User newUser);
    Task<User?> GetUserByEmailAsync(string email);
    UserDto GetUserDtoFromUser(User user);
    string CreateJwtToken(User user);
    Task<User> GetUserFromToken(string token);
}