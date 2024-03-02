using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
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
        DateTime createdAt = DateTime.Now;
        DateTime expiry = createdAt.AddHours(4);
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email)
        };
            
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var jsonToken = new JwtSecurityToken(
            claims: claims,
            notBefore: createdAt,
            expires: expiry,
            signingCredentials: credentials);
        
        Log.Information("Created JWT valid to: " + jsonToken.ValidTo + " valid from: " + jsonToken.ValidFrom);

        return new JwtSecurityTokenHandler().WriteToken(jsonToken);
    }

    public User? GetUserFromToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            if (jsonToken!.ValidTo < DateTime.Now)
            {
                throw new SecurityTokenExpiredException("Expired JWT valid to: " + jsonToken.ValidTo + " valid from: " + jsonToken.ValidFrom + " issued at: " + jsonToken.IssuedAt);
            }
        
            string email = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value!;

            return GetUserByEmailAsync(email).Result;
        }
        catch (Exception e)
        {
            Log.Error(e, "JWT Error");
            return null;
        }
        
    }
}