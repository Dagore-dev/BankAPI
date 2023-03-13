using BankAPI.Context;
using BankAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Services;

public class ClientService
{
  readonly BankContext context;
  public ClientService (BankContext context)
  {
    this.context = context;
  }
  public async Task<IEnumerable<Client>> GetAll () => await context.Clients.ToListAsync();
  public async Task<Client?> GetById (int id) => await context.Clients.FindAsync(id);
  public async Task<Client> Create (Client client)
  {
    context.Clients.Add(client);

    await context.SaveChangesAsync();

    return client;
  }
  public async void Update (int id, Client client, Client clientToUpdate)
  {    
    clientToUpdate.Name = client.Name;
    clientToUpdate.PhoneNumber = client.PhoneNumber;
    clientToUpdate.Email = client.Email;

    await context.SaveChangesAsync();
  }
  public async void Delete (Client client)
  {
    context.Clients.Remove(client);
    
    await context.SaveChangesAsync();
  }
  public async Task<bool> Exist (int id)
  {
    return await context.Clients.AnyAsync(c => c.Id == id);
  }
}
