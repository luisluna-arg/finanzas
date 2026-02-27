using System.Data.Common;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Finance.Persistence.Telemetry;

/// <summary>
/// EF Core interceptor that emits an OpenTelemetry span for every DB command
/// and logs a warning for commands that exceed the slow-query threshold.
/// </summary>
public class DbTelemetryInterceptor : DbCommandInterceptor
{
    private static readonly ActivitySource DbActivitySource = new("Finance.Api.Db");

    private readonly ILogger<DbTelemetryInterceptor> logger;
    private readonly TimeSpan threshold = TimeSpan.FromMilliseconds(500);

    public DbTelemetryInterceptor(ILogger<DbTelemetryInterceptor> logger)
    {
        this.logger = logger;
    }

    public override async ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            return await base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }
        finally
        {
            sw.Stop();
            Report(command, sw.Elapsed);
        }
    }

    public override async ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            return await base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
        }
        finally
        {
            sw.Stop();
            Report(command, sw.Elapsed);
        }
    }

    public override async ValueTask<InterceptionResult<object>> ScalarExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<object> result,
        CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            return await base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
        }
        finally
        {
            sw.Stop();
            Report(command, sw.Elapsed);
        }
    }

    private void Report(DbCommand command, TimeSpan elapsed)
    {
        // Start a short-lived activity for the command so it appears in traces
        using var activity = DbActivitySource.StartActivity("db.command", ActivityKind.Client);
        if (activity is not null)
        {
            activity.SetTag("db.statement", command.CommandText);
            activity.SetTag("db.duration_ms", elapsed.TotalMilliseconds);
            activity.SetTag("db.system", command.Connection?.GetType().Name ?? "unknown");
        }

        if (elapsed > threshold)
        {
            logger.LogWarning("Slow SQL {Elapsed}ms\n{Command}", elapsed.TotalMilliseconds, command.CommandText);
        }
    }
}
