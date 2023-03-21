using Microsoft.AspNetCore.Mvc;
using BankAPI.Models;
using BankAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace BankAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ClientController : ControllerBase
{
  readonly ClientService clientService;
  public ClientController(ClientService clientService)
  {
    this.clientService = clientService;
  }
  
  [HttpGet]
  public async Task<IEnumerable<Client>?> Get () => await clientService.GetAll();
  [HttpGet("{id}")]
  public async Task<ActionResult<Client>> GetById (int id)
  {
    Client? client = await clientService.GetById(id);
    if (client is null)
      return ClientNotFound(id);
    
    return client;
  }
  [Authorize(Policy = "SuperAdmin")]
  [HttpPost]
  public async Task<IActionResult> Create (Client client)
  {
    Client clientCreated = await clientService.Create(client);
    return CreatedAtAction(nameof(GetById), new { id = clientCreated.Id }, clientCreated);
  }
  [Authorize(Policy = "SuperAdmin")]
  [HttpPut("{id}")]
  public async Task<IActionResult> Update (int id, Client client)
  {
    if (id != client.Id)
      return BadRequest(new { message = $"The param Id ({id}) and the payload Id ({client.Id}) doesn't match" });
    
    Client? clientToUpdate = await clientService.GetById(id);
    if (clientToUpdate is null)
      return ClientNotFound(id);

    await clientService.Update(client, clientToUpdate);
    return NoContent();
  }
  [Authorize(Policy = "SuperAdmin")]
  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete (int id)
  {
    Client? clientToDelete = await clientService.GetById(id);
    if (clientToDelete is null)
      return ClientNotFound(id);

    await clientService.Delete(clientToDelete);
    return Ok();
  }
  NotFoundObjectResult ClientNotFound (int id)
  {
    return NotFound(new { message = $"Client with id = {id} not found" });
  }
}
