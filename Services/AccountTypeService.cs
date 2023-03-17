using BankAPI.Context;
using BankAPI.Models;

namespace BankAPI.Services;

public class AccountTypeService
{
  readonly BankContext context;
  public AccountTypeService(BankContext context)
  {
    this.context = context;
  }
  public async Task<AccountType?> GetById (int id) => await context.AccountTypes.FindAsync(id);
}