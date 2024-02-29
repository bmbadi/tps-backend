using System.ComponentModel.DataAnnotations;

namespace TPSBackend.Dtos;

public class UserCreateDto : UserLoginDto
{
    //[Required(ErrorMessage = "Name is required")]
    public string? Name { get; set; }
}