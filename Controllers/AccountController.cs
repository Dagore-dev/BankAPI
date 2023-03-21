using Microsoft.AspNetCore.Mvc;
using BankAPI.Services;
using BankAPI.Models;
using BankAPI.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace BankAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
  readonly ClientService clientService;
  readonly AccountService accountService;
  readonly AccountTypeService accountTypeService;
  public AccountController(ClientService clientService, AccountService accountService, AccountTypeService accountTypeService)
  {
    this.clientService = clientService;
    this.accountService = accountService;
    this.accountTypeService = accountTypeService;
  }
  [HttpGet]
  public async Task<IEnumerable<AccountResponseDTO>> Get () => await accountService.GetAll();
  [HttpGet("{id}")]
  public async Task<ActionResult<AccountResponseDTO?>> GetById (int id)
  {
    AccountResponseDTO? account = await accountService.GetDTOById(id);
    if (account is null)
      return AccountNotFound(id);
    
    return account;
  }
  [Authorize(Policy = "SuperAdmin")]
  [HttpPost]
  public async Task<IActionResult> Create (AccountRequestDTO account)
  {
    string validationResult = await ValidateAccount(account);
    if (validationResult != "Valid")
      return BadRequest(new { message = validationResult });

    Account accountCreated = await accountService.Create(account);
    return CreatedAtAction(nameof(GetById), new { id = accountCreated.Id }, accountCreated);
  }
  [Authorize(Policy = "SuperAdmin")]
  [HttpPut("{id}")]
  public async Task<IActionResult> Update (int id, AccountRequestDTO account)
  {
    if (id != account.Id)
      return BadRequest(new { message = "URI id and payload id doesn't match" });    
    
    Account? accountToUpdate = await accountService.GetById(id);
    if (accountToUpdate is null)
      return AccountNotFound(id);
    
    string validationResult = await ValidateAccount(account);
    if (validationResult != "Valid")
      return BadRequest(new { message = validationResult });
    
    accountService.Update(account, accountToUpdate);
    return NoContent();
  }
  [Authorize(Policy = "SuperAdmin")]
  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete (int id)
  {
    Account? accountToDelete = await accountService.GetById(id);
    if (accountToDelete is null)
      return AccountNotFound(id);
    
    accountService.Delete(accountToDelete);
    return Ok();
  }

  NotFoundObjectResult AccountNotFound(int id)
  {
    return NotFound(new { message = $"Account with id = {id} not found" });
  }
  async Task<string> ValidateAccount (AccountRequestDTO account)
  {
    AccountType? accountType = await accountTypeService.GetById(account.AccountType);
    if (accountType is null)
      return $"Given account type ({account.AccountType}) doesn't exist";
    
    int clientId = account.ClientId.GetValueOrDefault();
    Client? client = await clientService.GetById(clientId);
    if (client is null)
      return $"Client with id = {clientId} doesn't exist";

    return "Valid";
  }
}