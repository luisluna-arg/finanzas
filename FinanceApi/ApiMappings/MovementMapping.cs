using FinanceApi.Dtos;

namespace FinanceApi.ApiMappings;
internal static class MovementMappingExtensions
{
    private static string route = "/movement";
    internal static void MovementMapping(this WebApplication app)
    {
        app.BaseMapping(route, (FinanceDb db) => db.Movement);

        app.MapGet(route + "/totals", GetTotals);
    }

    private static IResult GetTotals(FinanceDb db)
    {
        var latestMovement = db.Movement.OrderByDescending(o => o.TimeStamp).FirstOrDefault();

        if (latestMovement == null) return Results.BadRequest<string>("No funds available");
        
        return Results.Ok<TotalsDto>(new TotalsDto());
    }
}
