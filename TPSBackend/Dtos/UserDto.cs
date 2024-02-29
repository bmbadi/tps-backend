namespace TPSBackend.Dtos;

public class UserDto
{
    public UserDto(string name, string email)
    {
        Name = name;
        Email = email;
    }

    public string Name { get; }
    public string Email { get; }
}