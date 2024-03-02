using System.Net;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using TPSBackend.Dtos;
using TPSBackend.Dtos.Atm;
using TPSBackend.Enums;
using TPSBackend.Models;
using TPSBackend.Services.Interfaces;

namespace TPSBackend.Controllers;

[ApiController]
[Route("api/atm")]
public class AtmController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAtmService _atmService;
    private readonly IUserAccountService _userAccountService;

    public AtmController(IUserService userService, IAtmService atmService, IUserAccountService userAccountService)
    {
        _userService = userService;
        _atmService = atmService;
        _userAccountService = userAccountService;
    }

    [HttpPost]
    public IActionResult AdminCreateAtm()
    {
        try
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            User? loggedInUser = _userService.GetUserFromToken(token);
            if (loggedInUser == null || loggedInUser.UserRole != UserRole.Admin)
            {
                ResponseMessage responseMessage = new ResponseMessage("Unauthorized", "Unauthorized");
                return Unauthorized(responseMessage);
            }

            Atm atm = new Atm
            {
                Balance = 0.0
            };

            if (_atmService.CreateAtm(atm).Result)
            {
                return StatusCode((int) HttpStatusCode.Created, _atmService.AtmToAtmDto(atm));
            }
            else
            {
                ResponseMessage responseMessage = new ResponseMessage("Error", "An error occurred while saving the ATM. Please try again later");
                return StatusCode((int) HttpStatusCode.InternalServerError, responseMessage);
            }
        }
        catch (Exception e)
        {
            string errorMessage = "An error occurred while creating the ATM. Please retry later.";
            Log.Error(e, errorMessage);
            ResponseMessage responseMessage = new ResponseMessage("Server Error", errorMessage);
            return StatusCode((int) HttpStatusCode.InternalServerError, responseMessage);
        }
    }

    private IActionResult AtmAdminTransferFunds(AtmAdminFundsTransferDto dto, TransactionType transactionType)
    {
        if (transactionType == TransactionType.Withdraw || transactionType ==TransactionType.Deposit)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            User? loggedInUser = _userService.GetUserFromToken(token);
            if (loggedInUser == null || loggedInUser.UserRole != UserRole.Admin)
            {
                ResponseMessage responseMessage = new ResponseMessage("Unauthorized", "Unauthorized");
                return Unauthorized(responseMessage);
            }

            if (dto.AtmId == null || dto.Amount == null)
            {
                ResponseMessage responseMessage = new ResponseMessage("Incomplete Data", "Kindly submit all the required data");
                return BadRequest(responseMessage);
            }
            
            if (dto.Amount <= 0)
            {
                string action = transactionType == TransactionType.Withdraw ? "withdraw" : "deposit";
                ResponseMessage responseMessage = new ResponseMessage("Invalid Amount", "You can only " + action + " funds greater than 0");
                return BadRequest(responseMessage);
            }

            Atm? atm = _atmService.GetAtmById((long) dto.AtmId).Result;

            if (atm == null)
            {
                ResponseMessage responseMessage = new ResponseMessage("Invalid ATM ID", "No ATM found with the submitted ID: " + dto.AtmId);
                return BadRequest(responseMessage);
            }

            double amt = (double) dto.Amount;
            if (transactionType == TransactionType.Withdraw)
            {
                if (atm.Balance < amt)
                {
                    ResponseMessage responseMessage = new ResponseMessage("Insufficient Funds", "The ATM does not have sufficient funds to complete the transaction. Please retry later.");
                    return BadRequest(responseMessage);
                }
            }

            double atmBalanceBefore = atm.Balance;
            double atmBalanceAfter;
            
            if (transactionType == TransactionType.Withdraw)
            {
                atmBalanceAfter = atm.Balance - amt;
            }
            else
            {
                atmBalanceAfter = atm.Balance + amt;
            }

            atm.Balance = atmBalanceAfter;

            AtmTransaction atmTransaction = new AtmTransaction
            {
                Amount = amt,
                AtmId = atm.AtmId,
                TransactionType = transactionType,
                TransactedById = loggedInUser.UserId,
                TransactedAt = DateTime.Now,
                UserTransactionId = null,
                BalanceBefore = atmBalanceBefore,
                BalanceAfter = atmBalanceAfter
            };
            
            bool a = _atmService.SaveAtmAdminTransferFundsRecords(atmTransaction, atm).Result;

            if (a)
            {
                string action = transactionType == TransactionType.Withdraw ? "withdrawn from" : "deposited to";
                ResponseMessage responseMessage = new ResponseMessage("Success", "Funds " + action + " the ATM successfully");
                return Ok(responseMessage);
            }
            else
            {
                string action = transactionType == TransactionType.Withdraw ? "withdrawing funds from the ATM" : "depositing funds into the ATM";
                ResponseMessage responseMessage = new ResponseMessage("Fail", "An error occurred while " + action);
                return StatusCode((int) HttpStatusCode.InternalServerError, responseMessage); 
            }
        }
        else
        {
            throw new Exception("Unsupported AtmAdminTransferFunds type");
        }
    }
  
    [HttpPost("admin/deposit")]
    public IActionResult AtmAdminDepositFunds([FromBody] AtmAdminFundsTransferDto dto)
    {
        try
        {
            return AtmAdminTransferFunds(dto, TransactionType.Deposit);
        }
        catch (Exception e)
        {
            string errorMessage = "An error occurred while admin depositing funds into the ATM. Please retry later.";
            Log.Error(e, errorMessage);
            ResponseMessage responseMessage = new ResponseMessage("Server Error", errorMessage);
            return StatusCode((int) HttpStatusCode.InternalServerError, responseMessage);
        }
    }
    
    
    [HttpPost("admin/withdraw")]
    public IActionResult AtmAdminWithdrawFunds([FromBody] AtmAdminFundsTransferDto dto)
    {
        try
        {
            return AtmAdminTransferFunds(dto, TransactionType.Withdraw);
        }
        catch (Exception e)
        {
            string errorMessage = "An error occurred while admin withdrawing funds from the ATM. Please retry later.";
            Log.Error(e, errorMessage);
            ResponseMessage responseMessage = new ResponseMessage("Server Error", errorMessage);
            return StatusCode((int) HttpStatusCode.InternalServerError, responseMessage);
        }
    }
     
    [HttpPost("user/withdraw")]
    public IActionResult AtmUserWithdrawFunds([FromBody] AtmUserWithdrawDto dto)
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
            
            if (dto.AtmId == null || dto.Amount == null || dto.AccountNumber == null)
            {
                ResponseMessage responseMessage = new ResponseMessage("Incomplete Data", "Kindly submit all the required data");
                return BadRequest(responseMessage);
            }
            
            if (dto.Amount <= 0)
            {
                ResponseMessage responseMessage = new ResponseMessage("Invalid Amount", "You can only withdraw funds greater than 0");
                return BadRequest(responseMessage);
            }
            
            double amtToWithdraw = (double) dto.Amount;
            UserAccount? userAccount = _userAccountService.GetUserAccountByAccountNumberAndUser((long) dto.AccountNumber, loggedInUser.UserId).Result;
            if (userAccount == null)
            {
                ResponseMessage responseMessage = new ResponseMessage("Invalid Account Number", "No account found with the submitted account number: " + dto.AccountNumber);
                return BadRequest(responseMessage);
            }
            
            if (userAccount.Balance < amtToWithdraw)
            {
                ResponseMessage responseMessage = new ResponseMessage("Insufficient Funds", "You do not have sufficient funds in your account to complete the transaction. Please retry later.");
                return BadRequest(responseMessage);
            }

            Atm? atm = _atmService.GetAtmById((long) dto.AtmId).Result;
            if (atm == null)
            {
                ResponseMessage responseMessage = new ResponseMessage("Invalid ATM ID", "No ATM found with the submitted ID: " + dto.AtmId);
                return BadRequest(responseMessage);
            }
            
            if (atm.Balance < amtToWithdraw)
            {
                ResponseMessage responseMessage = new ResponseMessage("Insufficient Funds", "The ATM does not have sufficient funds to complete the transaction. Please retry later.");
                return BadRequest(responseMessage);
            }
            
            double userAccountBalanceBefore = userAccount.Balance;
            double userAccountBalanceAfter = userAccount.Balance - amtToWithdraw;
            double atmBalanceBefore = atm.Balance;
            double atmBalanceAfter = atm.Balance - amtToWithdraw;
            
            atm.Balance = atmBalanceAfter;
            userAccount.Balance = userAccountBalanceAfter;
            
            AtmTransaction atmTransaction = new AtmTransaction
            {
                Amount = amtToWithdraw,
                AtmId = atm.AtmId,
                TransactionType = TransactionType.Withdraw,
                TransactedById = loggedInUser.UserId,
                TransactedAt = DateTime.Now,
                UserTransactionId = null,
                BalanceBefore = atmBalanceBefore,
                BalanceAfter = atmBalanceAfter
            };

            UserTransaction userTransaction = new UserTransaction
            {
                UserId = loggedInUser.UserId,
                TransactionType = TransactionType.Withdraw,
                Amount = amtToWithdraw,
                AccountFromId = userAccount.UserAccountId,
                AccountToId = null,
                BalanceBefore = userAccountBalanceBefore,
                BalanceAfter = userAccountBalanceAfter,
                TransactedAt = DateTime.Now
            };
            
            bool a = _atmService.SaveAtmUserWithdrawFundsRecords(atmTransaction, atm, userAccount, userTransaction).Result;

            if (a)
            {
                ResponseMessage responseMessage = new ResponseMessage("Success", "Funds withdrawn from the ATM successfully");
                return Ok(responseMessage);
            }
            else
            {
                ResponseMessage responseMessage = new ResponseMessage("Fail", "An error occurred while withdrawing funds from the ATM");
                return StatusCode((int) HttpStatusCode.InternalServerError, responseMessage); 
            }
        }
        catch (Exception e)
        {
            string errorMessage = "An error occurred while user withdrawing funds from the ATM. Please retry later.";
            Log.Error(e, errorMessage);
            ResponseMessage responseMessage = new ResponseMessage("Server Error", errorMessage);
            return StatusCode((int) HttpStatusCode.InternalServerError, responseMessage);
        }
    }
    
    [HttpGet]
    public IActionResult GetAllAtms()
    {
        try
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            User? loggedInUser = _userService.GetUserFromToken(token);
            if (loggedInUser == null || loggedInUser.UserRole != UserRole.Admin)
            {
                ResponseMessage responseMessage = new ResponseMessage("Unauthorized", "Unauthorized");
                return Unauthorized(responseMessage);
            }

            ICollection<Atm> allAtms = _atmService.GetAllAtms().Result;
            return Ok(_atmService.AtmToAtmDto(allAtms));
        }
        catch (Exception e)
        {
            string errorMessage = "An error occurred while getting the ATMs. Please retry later.";
            Log.Error(e, errorMessage);
            ResponseMessage responseMessage = new ResponseMessage("Server Error", errorMessage);
            return StatusCode((int) HttpStatusCode.InternalServerError, responseMessage);
        }
    }
    
    [HttpGet("{atmId}")]
    public IActionResult GetAtmById(long atmId)
    {
        try
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            User? loggedInUser = _userService.GetUserFromToken(token);
            if (loggedInUser == null || loggedInUser.UserRole != UserRole.Admin)
            {
                ResponseMessage responseMessage = new ResponseMessage("Unauthorized", "Unauthorized");
                return Unauthorized(responseMessage);
            }

            Atm? atm = _atmService.GetAtmById(atmId).Result;

            if (atm == null)
            {
                ResponseMessage responseMessage = new ResponseMessage("Invalid ATM ID", "No ATM found with the submitted id (" + atmId + ")");
                return BadRequest(responseMessage);
            }
            return Ok(_atmService.AtmToAtmDto(atm));
        }
        catch (Exception e)
        {
            string errorMessage = "An error occurred while getting the ATM. Please retry later.";
            Log.Error(e, errorMessage);
            ResponseMessage responseMessage = new ResponseMessage("Server Error", errorMessage);
            return StatusCode((int) HttpStatusCode.InternalServerError, responseMessage);
        }
    }
}