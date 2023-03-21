using Microsoft.AspNetCore.Mvc;
using BankAPI.Services;
using BankAPI.DTOs;
using BankAPI.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace BankAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController: ControllerBase
{
  readonly LoginService loginService;
  readonly IConfiguration config;
  public LoginController(LoginService loginService, IConfiguration config)
  {
    this.loginService = loginService;
    this.config = config;
  }
  [HttpPost]
  public async Task<IActionResult> Login (AdminRequestDTO adminRequestDTO)
  {
    Administrator? admin = await loginService.GetAdmin(adminRequestDTO);
    if (admin is null)
      return BadRequest(new { message = "Invalid credentials" });
    
    var token = GenerateToken(admin);
    return Ok(new { token = token });
  }
  string GenerateToken (Administrator admin)
  {
    var secret = config.GetSection("JWT:Key").Value;
    if (secret is null)
      throw new Exception("No secret key founded");
    
    var claims = new[]
    {
      new Claim(ClaimTypes.Name, admin.Name),
      new Claim(ClaimTypes.Email, admin.Email),
      new Claim("AdminType", admin.AdminType)
    };
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
    var securityToken = new JwtSecurityToken
    (
      claims: claims,
      expires: DateTime.Now.AddDays(1),
      signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(securityToken);
  }
}
