using System.Net;
using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using TPSBackend.Dtos;
using TPSBackend.Enums;
using TPSBackend.Models;
using TPSBackend.Services.Interfaces;

namespace TPSBackend.Controllers;

[ApiController]
[Route("api/transaction")]
public class TransactionController : ControllerBase
{
    private readonly IUserAccountService _userAccountService;
    private readonly ITransactionService _transactionService;
    private readonly IUserService _userService;

    public TransactionController(IUserAccountService userAccountService, ITransactionService transactionService, IUserService userService)
    {
        _userAccountService = userAccountService;
        _transactionService = transactionService;
        _userService = userService;
    }
    
    [HttpPost("transfer")]
    public IActionResult TransferFunds([FromBody] FundTransferRequestDto dto)
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
            
            if (dto.Amount == null || dto.AccountFrom == null || dto.AccountTo == null || dto.AccountFrom == dto.AccountTo)
            {
                ResponseMessage responseMessage = new ResponseMessage("Incomplete Data", "Kindly submit all the required data");
                return BadRequest(responseMessage);
            }

            UserAccount? accountFrom = _userAccountService.GetUserAccountByAccountNumberAndUser((long) dto.AccountFrom, loggedInUser.UserId).Result;
            if (accountFrom == null)
            {
                ResponseMessage responseMessage = new ResponseMessage("Invalid Account From", "The submitted account from is invalid");
                return BadRequest(responseMessage);
            }
            
            UserAccount? accountTo = _userAccountService.GetUserAccountByAccountNumber((long) dto.AccountTo).Result;
            if (accountTo == null)
            {
                ResponseMessage responseMessage = new ResponseMessage("Invalid Account To", "The submitted account to is invalid");
                return BadRequest(responseMessage);
            }

            if (dto.Amount <= 0)
            {
                ResponseMessage responseMessage = new ResponseMessage("Invalid Amount", "You can only transfer amounts greater than 0");
                return BadRequest(responseMessage);
            }

            if (accountFrom.Balance < dto.Amount)
            {
                ResponseMessage responseMessage = new ResponseMessage("Insufficient Balance", "Sorry, you do not have adequate funds to complete the transfer of funds.");
                return BadRequest(responseMessage);
            }

            double balanceBeforeFrom = accountFrom.Balance;
            double balanceBeforeTo = accountTo.Balance;
            double balanceAfterFrom = balanceBeforeFrom - (double) dto.Amount;
            double balanceAfterTo = balanceBeforeTo + (double) dto.Amount;

            UserTransaction userTransactionFrom = new UserTransaction
            {
                UserId = loggedInUser.UserId,
                TransactionType = TransactionType.Transfer,
                Amount = (double) dto.Amount,
                AccountFromId = accountFrom.UserAccountId,
                AccountToId = accountTo.UserAccountId,
                BalanceBefore = balanceBeforeFrom,
                BalanceAfter = balanceAfterFrom,
                TransactedAt = DateTime.Now
            };
            
            UserTransaction userTransactionTo = new UserTransaction
            {
                UserId = loggedInUser.UserId,
                TransactionType = TransactionType.Transfer,
                Amount = (double) dto.Amount,
                AccountFromId = accountFrom.UserAccountId,
                AccountToId = accountTo.UserAccountId,
                BalanceBefore = balanceBeforeTo,
                BalanceAfter = balanceAfterTo,
                TransactedAt = DateTime.Now
            };

            accountFrom.Balance = balanceAfterFrom;
            accountTo.Balance = balanceAfterTo;
            
            bool a = _transactionService.SaveFundsTransferRecords(userTransactionFrom, userTransactionTo, accountFrom, accountTo).Result;

            if (a)
            {
                ResponseMessage responseMessage = new ResponseMessage("Success", "Funds transferred successfully");
                return Ok(responseMessage);
            }
            else
            {
                ResponseMessage responseMessage = new ResponseMessage("Fail", "An error occurred while transferring the funds");
                return StatusCode((int) HttpStatusCode.InternalServerError, responseMessage); 
            }
        }
        catch (Exception e)
        {
            string errorMessage = "An error occurred while transferring funds. Please retry later.";
            Log.Error(e, errorMessage);
            ResponseMessage responseMessage = new ResponseMessage("Server Error", errorMessage);
            return StatusCode((int) HttpStatusCode.InternalServerError, responseMessage);
        }
    }
}