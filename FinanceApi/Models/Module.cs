namespace FinanceApi.Models;

internal class Module : Entity
{
    required public string Name { get; set; } = string.Empty;
    required public DateTime CreatedAt { get; set; }
    required public Currency Currency { get; set; }
}
