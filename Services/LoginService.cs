using BankAPI.Context;
using BankAPI.DTOs;
using BankAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Services;

public class LoginService
{
  readonly BankContext context;
  public LoginService(BankContext context)
  {
    this.context = context;
  }
  public async Task<Administrator?> GetAdmin (AdminRequestDTO adminRequestDTO)
  {
    return await context.Administrators.SingleOrDefaultAsync(admin => admin.Email == adminRequestDTO.Email && admin.Password == adminRequestDTO.Password);
  }
}
