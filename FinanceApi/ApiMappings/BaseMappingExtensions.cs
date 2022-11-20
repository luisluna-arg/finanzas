namespace FinanceApi.ApiMappings;

using FinanceApi.Models;
using Microsoft.EntityFrameworkCore;

internal static class BaseMappingExtensions
{
    internal static void BaseMapping<TEntity>(this WebApplication app, string route, Func<FinanceDb, DbSet<TEntity>> dbSetMapper)
        where TEntity : Entity
    {
        app.MapGet(route + "/all", async (FinanceDb db) =>
            await dbSetMapper(db).ToListAsync())
            .WithOpenApi();

        app.MapGet(route + "/single/{id}", async (int id, FinanceDb db) =>
            await dbSetMapper(db).FindAsync(id) is TEntity entity ? Results.Ok(entity) : Results.NotFound())
            .WithOpenApi();

        app.MapPost(route, async (TEntity entity, FinanceDb db) =>
        {
            dbSetMapper(db).Add(entity);
            await db.SaveChangesAsync();

            return Results.Created($"" + route + "/{bank.Id}", entity);
        }).WithOpenApi();

        app.MapPut(route + "/{id}", async (int id, TEntity inputEntity, FinanceDb db) =>
        {
            var bank = await dbSetMapper(db).FindAsync(id);

            if (bank is null) return Results.NotFound();

            bank.Update(inputEntity);

            await db.SaveChangesAsync();

            return Results.NoContent();
        }).WithOpenApi();

        app.MapDelete(route + "/{id}", async (int id, FinanceDb db) =>
        {
            var dbSet = dbSetMapper(db);
            if (await dbSet.FindAsync(id) is TEntity entity)
            {
                dbSet.Remove(entity);
                await db.SaveChangesAsync();
                return Results.Ok(entity);
            }

            return Results.NotFound();
        }).WithOpenApi();
    }
}
