using System.ComponentModel.DataAnnotations;
using Finance.Domain.Models.Base;

namespace Finance.Domain.Models.Currencies;

public class CurrencySymbol() : Entity<Guid>()
{
    public Guid CurrencyId { get; set; }
    public virtual Currency Currency { get; set; } = default!;
    [Required]
    [MaxLength(10)]
    required public string Symbol { get; set; }

    public static CurrencySymbol Default(string? symbol = null)
        => new CurrencySymbol() { Symbol = symbol ?? string.Empty };
}
