using TPSBackend.Models;

namespace TPSBackend.Services.Interfaces;

public interface IUserService
{
    Task<bool> CreateUserAsync(User newUser);
    Task<User?> GetUserByEmailAsync(string email);
}