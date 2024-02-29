using Microsoft.EntityFrameworkCore;
using TPSBackend.Data;
using TPSBackend.Dtos;
using TPSBackend.Models;
using TPSBackend.Services.Interfaces;

namespace TPSBackend.Services;

public class UserService : IUserService
{
    private readonly DataContext _context;

    public UserService(DataContext context)
    {
        _context = context;
    }
    
    public async Task<bool> CreateUserAsync(User newUser)
    {
        _context.Users.Add(newUser);
        int res = await _context.SaveChangesAsync();
        return res == 1;
    }
    
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public UserDto GetUserDtoFromUser(User user)
    {
        return new UserDto(user.Name, user.Email);
    }
}