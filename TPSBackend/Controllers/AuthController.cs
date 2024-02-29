using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TPSBackend.Dtos;
using TPSBackend.Models;
using TPSBackend.Repositories.Interfaces;
using TPSBackend.Services;
using TPSBackend.Services.Interfaces;
using TPSBackend.Utils;

namespace TPSBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost(Name = "Register")]
    [Route("register")]
    public IActionResult CreateUser([FromBody] UserCreateDto userCreateDto)
    {
        //todo check if is valid email
        if (userCreateDto.Name.IsNullOrEmpty() || userCreateDto.Email.IsNullOrEmpty() 
            || userCreateDto.Password.IsNullOrEmpty())
        {
            ResponseMessage responseMessage = new ResponseMessage("Incomplete Data", "Kindly submit all the required data");
            return BadRequest(responseMessage);
        }

        User? existingUser = _userService.GetUserByEmailAsync(userCreateDto.Email!).Result;
        if (existingUser != null)
        {
            ResponseMessage responseMessage = new ResponseMessage("User Exists", "A user with the submitted email address already exists. Kindly use a different email address or proceed to login.");
            return BadRequest(responseMessage);
        }

        User userToCreate = new User
        {
            Name = userCreateDto.Name!,
            Email = userCreateDto.Email!,
            Password = SecurePasswordHasher.Hash(userCreateDto.Password!)
        };

        if (!_userService.CreateUserAsync(userToCreate).Result)
        {
            ResponseMessage responseMessage = new ResponseMessage("Server Error", "An error occurred while saving the user. Please retry later.");
            return StatusCode((int) HttpStatusCode.InternalServerError, responseMessage);
        }
       
        //todo return user object, with JWT
        ResponseMessage responseMessageSuccess = new ResponseMessage("Success", "User created successfully");
        return StatusCode((int) HttpStatusCode.Created, responseMessageSuccess);
    }

    [HttpPost(Name = "Login")]
    [Route("login")]
    public IActionResult LoginUser(UserLoginDto userLoginDto)
    {
        //todo check if is valid email
        if (userLoginDto.Email.IsNullOrEmpty() || userLoginDto.Password.IsNullOrEmpty())
        {
            ResponseMessage responseMessage = new ResponseMessage("Incomplete Data", "Kindly submit all the required data");
            return BadRequest(responseMessage);
        }
        
        User? existingUser = _userService.GetUserByEmailAsync(userLoginDto.Email!).Result;
        if (existingUser == null)
        {
            ResponseMessage responseMessage = new ResponseMessage("Invalid Credentials", "Invalid email or password");
            return BadRequest(responseMessage);
        }

        if (!SecurePasswordHasher.Verify(userLoginDto.Password!, existingUser.Password))
        {
            ResponseMessage responseMessage = new ResponseMessage("Invalid Credentials", "Invalid email or password");
            return BadRequest(responseMessage);
        }

        UserDto userDto = _userService.GetUserDtoFromUser(existingUser);
        return Ok(userDto);
    }
}