using FinanceApi.Domain.Models.Base;

namespace FinanceApi.Domain.Models;

public class AppModule : Entity
{
    required public string Name { get; set; } = string.Empty;
    required public DateTime CreatedAt { get; set; }
    required public Currency Currency { get; set; }
    public ICollection<Movement> Movements { get; set; } = new List<Movement>();

    public static AppModule Default()
    {
        return new AppModule()
        {
            Name = string.Empty,
            CreatedAt = DateTime.UtcNow,
            Currency = Currency.Default()
        };
    }
}
