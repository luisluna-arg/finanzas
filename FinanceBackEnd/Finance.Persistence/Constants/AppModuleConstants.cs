using Finance.Domain.Enums;

namespace Finance.Persistence.Constants;

public static class AppModuleConstants
{
    public const string FundsId = "f92f45fe-1c9e-4b65-b32b-b033212a7b27";
    public const string DollarFundsId = "93c77ebf-b726-4148-aebe-1e11abc7b47f";
    public const string DebitsId = "4c1ee918-e8f9-4bed-8301-b4126b56cfc0";
    public const string DollarDebitsId = "03cc66c7-921c-4e05-810e-9764cd365c1d";
    public const string IOLInvestmentsId = "65325dbb-13b0-44ff-82ad-5808a26581a4";

    private static readonly Dictionary<string, AppModuleTypeEnum> TypesValue = new Dictionary<string, AppModuleTypeEnum>()
    {
        { FundsId, AppModuleTypeEnum.Funds },
        { DollarFundsId, AppModuleTypeEnum.Funds },
        { DebitsId, AppModuleTypeEnum.Debits },
        { DollarDebitsId, AppModuleTypeEnum.Debits },
        { IOLInvestmentsId, AppModuleTypeEnum.Investments }
    };

    private static readonly Dictionary<string, string> NamesValue = new Dictionary<string, string>()
    {
        { FundsId, "Fondos" },
        { DollarFundsId, "Fondos dólares" },
        { DebitsId, "Débitos" },
        { DollarDebitsId, "Débitos en dólares" },
        { IOLInvestmentsId, "Inversiones IOL" }
    };

    public static Dictionary<string, string> Names => NamesValue;

    public static Dictionary<string, AppModuleTypeEnum> Types => TypesValue;

    public static string[][] AppModuleCurrencyPairs =>
    [
        [FundsId, CurrencyConstants.PesoId],
        [DollarFundsId, CurrencyConstants.DollarId],
        [DebitsId, CurrencyConstants.PesoId],
        [DollarDebitsId, CurrencyConstants.DollarId],
        [IOLInvestmentsId, CurrencyConstants.PesoId]
    ];
}