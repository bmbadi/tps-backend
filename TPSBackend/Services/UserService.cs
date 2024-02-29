using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TPSBackend.Data;
using TPSBackend.Dtos;
using TPSBackend.Models;
using TPSBackend.Services.Interfaces;

namespace TPSBackend.Services;

public class UserService : IUserService
{
    private readonly DataContext _context;
    private readonly IConfiguration _configuration;

    public UserService(DataContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
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
        return new UserDto(user.Name, user.Email, CreateJwtToken(user));
    }
    
    public string CreateJwtToken(User user)
    {
        DateTime expiry = DateTime.Now.AddHours(3);
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email)
        };
            
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiry,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public Task<User> GetUserFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
        string email = jsonToken!.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value!;

        return GetUserByEmailAsync(email)!;
    }
}