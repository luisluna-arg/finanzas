// Load all OTel packages via createRequire so Vite never tries to bundle them.
// These are CJS-only packages (sdk-node, exporter) that break under Vite's ESM bundler.
import { createRequire } from 'module';

const require = createRequire(import.meta.url);
const enabled = process.env.OTEL_ENABLED === 'true';

if (enabled) {
  const endpoint = process.env.OTEL_EXPORTER_OTLP_ENDPOINT ?? 'http://localhost:4318';

  // eslint-disable-next-line @typescript-eslint/no-require-imports
  const { NodeSDK } = require('@opentelemetry/sdk-node');
  // eslint-disable-next-line @typescript-eslint/no-require-imports
  const { OTLPTraceExporter } = require('@opentelemetry/exporter-trace-otlp-http');
  // eslint-disable-next-line @typescript-eslint/no-require-imports
  const { HttpInstrumentation } = require('@opentelemetry/instrumentation-http');
  // eslint-disable-next-line @typescript-eslint/no-require-imports
  const { UndiciInstrumentation } = require('@opentelemetry/instrumentation-undici');
  // eslint-disable-next-line @typescript-eslint/no-require-imports
  const { resourceFromAttributes } = require('@opentelemetry/resources');

  const sdk = new NodeSDK({
    resource: resourceFromAttributes({ 'service.name': 'Finance.Frontend' }),
    traceExporter: new OTLPTraceExporter({ url: `${endpoint}/v1/traces` }),
    instrumentations: [
      new HttpInstrumentation({
        ignoreIncomingRequestHook: (req: { url?: string }) => {
          const url = req.url ?? '';
          return (
            url.startsWith('/_src') ||
            url.startsWith('/favicon') ||
            url.startsWith('/health')
          );
        },
      }),
      new UndiciInstrumentation(),
    ],
  });

  sdk.start();

  process.on('SIGTERM', () => sdk.shutdown().catch(console.error));
}
