using Finance.Persistence.Telemetry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Finance.Application.Telemetry;

public static class TelemetryExtensions
{
    /// <summary>
    /// Registers OpenTelemetry tracing and the DB telemetry interceptor.
    /// Activation is controlled by a feature flag resolved in the following order:
    ///   1. The <c>OTEL_ENABLED</c> environment variable ("true"/"false").
    ///   2. The "OpenTelemetry:Enabled" configuration key (e.g. OpenTelemetry__Enabled env var).
    /// The OTLP endpoint is read from "OpenTelemetry:OtlpEndpoint" or the
    /// OTEL_EXPORTER_OTLP_ENDPOINT environment variable (default: http://localhost:4317).
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="configureTracing">
    /// Optional callback to add host-specific instrumentation (e.g. <c>AddAspNetCoreInstrumentation</c>
    /// for web projects). Defaults to a no-op so non-web hosts don't need to reference
    /// ASP.NET Core packages.
    /// </param>
    /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddTelemetry(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<TracerProviderBuilder>? configureTracing = null)
    {
        var enabled = ResolveEnabled(configuration);
        if (!enabled)
        {
            return services;
        }

        services.AddSingleton<DbTelemetryInterceptor>();

        services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Finance.Api"))
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddSource("Finance.Api.Db")
                    .AddSource("Finance.Api.Dispatcher")
                    .AddOtlpExporter(otlp =>
                    {
                        var endpoint = configuration["OpenTelemetry:OtlpEndpoint"]
                            ?? Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT")
                            ?? "http://localhost:4317";
                        otlp.Endpoint = new Uri(endpoint);
                    });

                configureTracing?.Invoke(tracing);
            });

        return services;
    }

    private static bool ResolveEnabled(IConfiguration configuration)
    {
        // 1. Explicit OTEL_ENABLED environment variable takes priority (consistent with frontend)
        var otelEnabledEnv = Environment.GetEnvironmentVariable("OTEL_ENABLED");
        if (!string.IsNullOrWhiteSpace(otelEnabledEnv))
        {
            return string.Equals(otelEnabledEnv.Trim(), "true", StringComparison.OrdinalIgnoreCase);
        }

        // 2. Fall back to the configuration key (supports OpenTelemetry__Enabled env var via ASP.NET Core convention)
        return configuration.GetValue("OpenTelemetry:Enabled", defaultValue: false);
    }
}
