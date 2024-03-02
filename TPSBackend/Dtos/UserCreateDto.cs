using System.ComponentModel.DataAnnotations;
using TPSBackend.Enums;

namespace TPSBackend.Dtos;

public class UserCreateDto : UserLoginDto
{
    //[Required(ErrorMessage = "Name is required")]
    public string? Name { get; set; }
    public UserRole? UserRole { get; set; }
}