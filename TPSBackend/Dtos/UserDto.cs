namespace TPSBackend.Dtos;

public class UserDto
{
    public UserDto(string name, string email, string token)
    {
        Name = name;
        Email = email;
        Token = token;
    }

    public string Name { get; }
    public string Email { get; }
    public string Token { get; }
}