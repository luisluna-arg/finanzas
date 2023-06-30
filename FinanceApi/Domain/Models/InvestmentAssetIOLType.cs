using System.ComponentModel.DataAnnotations;
using FinanceApi.Domain.Enums;

namespace FinanceApi.Domain.Models;

public class InvestmentAssetIOLType
{
    public InvestmentAssetIOLType()
    {
    }

    public InvestmentAssetIOLType(InvestmentAssetIOLTypeEnum enumValue)
    {
        this.Id = (ushort)enumValue;
        this.Name = enumValue.ToString();
    }

    [Key]
    required public ushort Id { get; set; } = 0;

    [Required]
    [MaxLength(100)]
    required public string Name { get; set; }
}
