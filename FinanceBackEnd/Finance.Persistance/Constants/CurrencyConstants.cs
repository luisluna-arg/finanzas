namespace Finance.Persistance.Constants;

public static class CurrencyConstants
{
    public const string PesoId = "6d189135-7040-45a1-b713-b1aa6cad1720";
    public const string DollarId = "efbf50bc-34d4-43e9-96f9-9f6213ea11b5";

    private static readonly Dictionary<string, string> NamesValue = new Dictionary<string, string>()
    {
        { PesoId, "Peso" },
        { DollarId, "Dollar" },
    };

    public static Dictionary<string, string> Names => NamesValue;

    public static string[] CurrencyIds => new string[] { PesoId, DollarId };
}

