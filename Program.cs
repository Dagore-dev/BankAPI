using BankAPI.Context;
using BankAPI.Services;

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
    builder.Services.AddSwaggerGen();
    builder.Services.AddSqlServer<BankContext>(builder.Configuration.GetConnectionString("BankConnection"));
    builder.Services.AddScoped<ClientService>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
      app.UseSwagger();
      app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
  }
}