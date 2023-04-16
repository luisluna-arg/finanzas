using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceApi.Models;

internal class CurrencyConversion : Entity
{
    public CurrencyConversion()
        : base()
    {
    }

    [ForeignKey("MovementId")]
    public Guid MovementId { get; set; }
    public Movement? Movement { get; internal set; }
    public decimal Amount { get; internal set; }
    public Currency? Currency { get; internal set; }
}
