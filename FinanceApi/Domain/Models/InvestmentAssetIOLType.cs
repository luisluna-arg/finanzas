using System.ComponentModel.DataAnnotations;
using FinanceApi.Domain.Enums;
using FinanceApi.Domain.Models.Base;

namespace FinanceApi.Domain.Models;

public class InvestmentAssetIOLType : Entity<ushort>
{
    public InvestmentAssetIOLType()
    {
    }

    public InvestmentAssetIOLType(InvestmentAssetIOLTypeEnum enumValue)
    {
        this.Id = (ushort)enumValue;
        this.Name = enumValue.ToString();
    }

    [Required]
    [MaxLength(100)]
    required public string Name { get; set; }
}
