using System.ComponentModel.DataAnnotations;
using Finance.Domain.Enums;
using Finance.Domain.Models.Base;

namespace Finance.Domain.Models;

public class IOLInvestmentAssetType : Entity<ushort>
{
    public static readonly string Default = "Default";

    public IOLInvestmentAssetType()
    {
    }

    public IOLInvestmentAssetType(IOLInvestmentAssetTypeEnum enumValue)
    {
        this.Id = (ushort)enumValue;
        this.Name = enumValue.ToString();
    }

    [Required]
    [MaxLength(100)]
    required public string Name { get; set; }
}
