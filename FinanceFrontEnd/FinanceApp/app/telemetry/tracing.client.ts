import { resourceFromAttributes } from '@opentelemetry/resources';
import { WebTracerProvider } from '@opentelemetry/sdk-trace-web';
import { BatchSpanProcessor } from '@opentelemetry/sdk-trace-base';
import { OTLPTraceExporter } from '@opentelemetry/exporter-trace-otlp-http';
import { DocumentLoadInstrumentation } from '@opentelemetry/instrumentation-document-load';
import { FetchInstrumentation } from '@opentelemetry/instrumentation-fetch';
import { registerInstrumentations } from '@opentelemetry/instrumentation';

declare global {
  interface Window {
    __OTEL?: { enabled: boolean; httpEndpoint: string };
  }
}

const config = typeof window !== 'undefined' ? window.__OTEL : undefined;

if (config?.enabled) {
  const provider = new WebTracerProvider({
    resource: resourceFromAttributes({ 'service.name': 'Finance.Frontend.Browser' }),
    spanProcessors: [
      new BatchSpanProcessor(
        new OTLPTraceExporter({ url: `${config.httpEndpoint}/v1/traces` }),
      ),
    ],
  });

  provider.register();

  registerInstrumentations({
    tracerProvider: provider,
    instrumentations: [
      new DocumentLoadInstrumentation(),
      new FetchInstrumentation(),
    ],
  });
}
