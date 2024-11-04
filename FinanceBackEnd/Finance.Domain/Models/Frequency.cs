using Finance.Domain.Enums;
using Finance.Domain.Models.Base;

namespace Finance.Domain.Models;

public class Frequency : Entity<FrequencyEnum>
{
    required public string Name { get; set; }
}