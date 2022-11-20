namespace FinanceApi.ApiMappings;
internal static class MappingExtensions
{
    internal static void Mappings(this WebApplication app)
    {
        app.BankMapping();
        app.CurrencyMapping();
        app.FundMapping();
        app.ModuleMapping();
        app.MovementMapping();
    }
}
