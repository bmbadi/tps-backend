using TPSBackend.Models;

namespace TPSBackend.Repositories.Interfaces;

public interface IUserRepository
{
    User? GetUserByEmail(string email);
    bool CreateUser(User user);
}