using FinanceApi.Domain.Models.Base;

namespace FinanceApi.Domain.Models;

public class Frequency : Entity<FrequencyEnum>
{
    required public string Name { get; set; }
}