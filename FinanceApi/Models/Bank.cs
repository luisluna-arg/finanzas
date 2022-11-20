namespace FinanceApi.Models;

using System.ComponentModel.DataAnnotations.Schema;

internal class Bank : Entity
{
    required public string? Name { get; set; }
}
