namespace Finance.Persistence.Constants;

public static class CurrencyConstants
{
    public const string PesoId = "6d189135-7040-45a1-b713-b1aa6cad1720";
    public const string DollarId = "efbf50bc-34d4-43e9-96f9-9f6213ea11b5";
    public const string DefaultCurrencyId = PesoId;

    private static readonly Dictionary<string, string> NamesValue = new()
    {
        { PesoId, "Peso" },
        { DollarId, "Dollar" },
    };

    private static readonly Dictionary<string, string> ShortNamesValue = new()
    {
        { PesoId, "ARS" },
        { DollarId, "USD" },
    };

    public static Dictionary<string, string> Names => NamesValue;

    public static Dictionary<string, string> ShortNames => ShortNamesValue;

    public static string[] CurrencyIds => [PesoId, DollarId];

    public static readonly Dictionary<string, Tuple<Guid, string>[]> CurrencySymbols = new()
    {
        {PesoId, [new (Guid.Parse("31d219c8-90a2-437b-8e52-f5fbf3bbd24f"), "$")]},
        {DollarId, [
            new (Guid.Parse("0a3d9502-aeec-4c35-92c1-9dc36c40612f"), "USD"),
            new (Guid.Parse("9b0ddd93-b13c-4443-ba97-0996672cbc1a"), "U$D"),
            new (Guid.Parse("9db01d76-76b0-438a-a6f3-38c4dda33ff4"), "US$")]}
    };
}

