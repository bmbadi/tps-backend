using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using TPSBackend.Dtos;
using TPSBackend.Models;
using TPSBackend.Services.Interfaces;

namespace TPSBackend.Controllers;

[ApiController]
[Route("api/userAccount")]
public class UserAccountController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IUserAccountService _userAccountService;

    public UserAccountController(IUserService userService, IUserAccountService userAccountService)
    {
        _userService = userService;
        _userAccountService = userAccountService;
    }

    [HttpGet]
    public IActionResult GetUserAccounts()
    {
        try
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            User? loggedInUser = _userService.GetUserFromToken(token);
            if (loggedInUser == null)
            {
                ResponseMessage responseMessage = new ResponseMessage("Unauthorized", "Unauthorized");
                return Unauthorized(responseMessage);
            }

            return Ok(_userAccountService.GetUserAccountDtoFromUser(loggedInUser));

        }
        catch (Exception e)
        {
            string errorMessage = "An error occurred while getting the user accounts. Please retry later.";
            Log.Error(e, errorMessage);
            ResponseMessage responseMessage = new ResponseMessage("Server Error", errorMessage);
            return StatusCode((int) HttpStatusCode.InternalServerError, responseMessage);
        }
    }
}