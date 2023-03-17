using BankAPI.Context;
using BankAPI.DTOs;
using BankAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Services;


public class AccountService
{
  readonly BankContext context;
  public AccountService (BankContext context)
  {
    this.context = context;
  }
  public async Task<IEnumerable<AccountResponseDTO>> GetAll ()
  {
    return await context.Accounts.Select(account => new AccountResponseDTO()
    {
      Id = account.Id,
      AccountName = account.AccountTypeNavigation.Name,
      ClientName = account.Client != null ? account.Client.Name : "" ,
      Balance = account.Balance,
      RegDate = account.RegDate
    }).ToListAsync();
  }
    public async Task<AccountResponseDTO?> GetDTOById (int id)
  {
    return await context.Accounts
      .Where(account => account.Id == id)
      .Select(account => new AccountResponseDTO()
      {
        Id = account.Id,
        AccountName = account.AccountTypeNavigation.Name,
        ClientName = account.Client != null ? account.Client.Name : "" ,
        Balance = account.Balance,
        RegDate = account.RegDate
      }).SingleOrDefaultAsync();
  }
  public async Task<Account?> GetById (int id) => await context.Accounts.FindAsync(id);
  public async Task<Account> Create (AccountRequestDTO account)
  {
    Account newAccount = new Account()
    {
      Id = account.Id,
      AccountType = account.AccountType,
      ClientId = account.ClientId,
      Balance = account.Balance
    };
    
    context.Accounts.Add(newAccount);
    await context.SaveChangesAsync();

    return newAccount;
  }
  public async void Update (AccountRequestDTO account, Account accountToUpdate)
  {
    accountToUpdate.AccountType = account.AccountType;
    accountToUpdate.ClientId = account.ClientId;
    accountToUpdate.Balance = account.Balance;

    await context.SaveChangesAsync();
  }
  public async void Delete (Account account)
  {
    context.Accounts.Remove(account);
    await context.SaveChangesAsync();
  }
}
