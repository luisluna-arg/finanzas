using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceApi.Domain.Models;

public class CurrencyConversion : Entity
{
    public CurrencyConversion()
        : base()
    {
    }

    [ForeignKey("MovementId")]
    public Guid MovementId { get; set; }
    public Movement? Movement { get; set; }
    public decimal Amount { get; set; }
    public Currency? Currency { get; set; }
}
