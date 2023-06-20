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
        this.Id = (short)enumValue;
        this.Name = enumValue.ToString();
    }

    [Key]
    required public short Id { get; set; } = 0;

    [Required]
    [MaxLength(100)]
    required public string Name { get; set; }
}
