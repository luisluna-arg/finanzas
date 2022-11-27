namespace FinanceApi.Models;

using System.ComponentModel.DataAnnotations.Schema;

internal class Bank : Entity
{
    public Bank()
        : base()
    {
    }

    required public string? Name { get; set; }
}
