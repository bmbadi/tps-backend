using System.ComponentModel.DataAnnotations;

namespace TPSBackend.Dtos;

public class UserCreateDto
{
    //[Required(ErrorMessage = "Name is required")]
    public string? Name { get; set; }
    
    //[Required(ErrorMessage = "Email is required")]
    public string? Email { get; set; }
    
    //[Required(ErrorMessage = "Password is required")]
    //[DataType(DataType.Password)]
    public string? Password { get; set; }
}