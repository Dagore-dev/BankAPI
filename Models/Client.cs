using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BankAPI.Models;

public partial class Client
{
    public int Id { get; set; }

    [MaxLength(200)]
    public string Name { get; set; } = null!;

    [MaxLength(40)]
    public string PhoneNumber { get; set; } = null!;

    [MaxLength(40)]
    [EmailAddress]
    public string? Email { get; set; }

    public DateTime RegDate { get; set; }

    [JsonIgnore]
    public virtual ICollection<Account> Accounts { get; } = new List<Account>();
}
