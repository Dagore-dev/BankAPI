using System.Text;
using BankAPI.Context;
using BankAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

internal class Program
{
  private static void Main(string[] args)
  {
    // DB Scaffold
    // dotnet ef dbcontext scaffold "Server=DESKTOP-57EPPU2\SQLEXPRESS;Database=Bank;Integrated Security=SSPI;Trust Server Certificate=true;" Microsoft.EntityFrameworkCore.SqlServer --context-dir .\Context --output-dir .\Models\BankModels
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(option =>
    {
        option.SwaggerDoc("v1", new OpenApiInfo { Title = "Bank API", Version = "v1" });
        option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });
        option.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type=ReferenceType.SecurityScheme,
                        Id="Bearer"
                    }
                },
                new string[]{}
            }
        });
    });
    builder.Services.AddSqlServer<BankContext>(builder.Configuration.GetConnectionString("BankConnection"));
    builder.Services.AddScoped<ClientService>();
    builder.Services.AddScoped<AccountService>();
    builder.Services.AddScoped<AccountTypeService>();
    builder.Services.AddScoped<LoginService>();
    
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => 
    {
      var secret = builder.Configuration.GetSection("JWT:Key").Value;
      if (secret is null)
        throw new Exception("No secret key founded");
      
      options.TokenValidationParameters = new TokenValidationParameters{
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
        ValidateIssuer = false,
        ValidateAudience = false
      };
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
      app.UseSwagger();
      app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
  }
}