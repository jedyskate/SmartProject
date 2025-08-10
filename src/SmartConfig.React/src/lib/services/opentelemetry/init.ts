import { WebTracerProvider } from '@opentelemetry/sdk-trace-web';
import { SimpleSpanProcessor, ConsoleSpanExporter } from '@opentelemetry/sdk-trace-base';
import { OTLPTraceExporter } from '@opentelemetry/exporter-trace-otlp-proto';
import { detectResources, resourceFromAttributes, type Resource } from '@opentelemetry/resources';
import { registerInstrumentations } from '@opentelemetry/instrumentation';
import { FetchInstrumentation } from '@opentelemetry/instrumentation-fetch';
import { DocumentLoadInstrumentation } from '@opentelemetry/instrumentation-document-load';
import { UserInteractionInstrumentation } from '@opentelemetry/instrumentation-user-interaction';
import { MeterProvider, PeriodicExportingMetricReader } from '@opentelemetry/sdk-metrics';
import { OTLPMetricExporter } from '@opentelemetry/exporter-metrics-otlp-proto';
import { metrics, type Span } from '@opentelemetry/api';
import { logs, SeverityNumber } from '@opentelemetry/api-logs';
import { LoggerProvider, SimpleLogRecordProcessor, ConsoleLogRecordExporter } from '@opentelemetry/sdk-logs';
import { OTLPLogExporter } from '@opentelemetry/exporter-logs-otlp-proto';

// Utility: Generate UUID for correlationId
function generateCorrelationId(): string {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        const r = Math.random() * 16 | 0;
        const v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

export async function initOpenTelemetry() {
    const baseUrl = import.meta.env.VITE_OTEL_EXPORTER_OTLP_ENDPOINT;
    const otlpHeaders = import.meta.env.VITE_OTEL_EXPORTER_OTLP_HEADERS;

    const headers: Record<string, string> = {};
    if (otlpHeaders) {
        const parts = otlpHeaders.split('=');
        if (parts.length === 2) {
            headers[parts[0]] = parts[1];
        }
    }

    const correlationId = generateCorrelationId(); // Generate once for this session

    // --- Traces Setup ---
    const exporter = new OTLPTraceExporter({
        url: `${baseUrl}/v1/traces`,
        headers,
    });

    const detected: Resource = await detectResources();
    const custom: Resource = resourceFromAttributes({
        'service.name': 'react-app',
        'service.version': '1.0.0',
    });

    const provider = new WebTracerProvider({
        resource: detected.merge(custom),
        spanProcessors: [
            new SimpleSpanProcessor(exporter),
            new SimpleSpanProcessor(new ConsoleSpanExporter())
        ]
    });
    provider.register();

    registerInstrumentations({
        instrumentations: [
            new DocumentLoadInstrumentation(),
            new UserInteractionInstrumentation(),
            new FetchInstrumentation({
                propagateTraceHeaderCorsUrls: /.*/, // optional
                applyCustomAttributesOnSpan: (span: Span) => {
                    span.setAttribute('correlationId', correlationId);
                },
            }),
        ],
    });

    // --- Metrics Setup ---
    const metricExporter = new OTLPMetricExporter({
        url: `${baseUrl}/v1/metrics`,
        headers,
    });

    const metricReader = new PeriodicExportingMetricReader({
        exporter: metricExporter,
        exportIntervalMillis: 10000,
    });

    const meterProvider = new MeterProvider({
        resource: detected.merge(custom),
        readers: [metricReader]
    });

    metrics.setGlobalMeterProvider(meterProvider);

    const meter = meterProvider.getMeter('react-app-metrics');

    const clickCounter = meter.createCounter('button_click_total', {
        description: 'Counts the total number of button clicks',
        unit: '1',
    });

    setInterval(() => {
        clickCounter.add(1, { component: 'main-button', userType: 'authenticated', correlationId });
    }, 5000);

    const activeUsersGauge = meter.createUpDownCounter('active_users', {
        description: 'Number of currently active users',
        unit: '1',
    });

    let currentActiveUsers = 0;
    setInterval(() => {
        if (Math.random() > 0.5) {
            activeUsersGauge.add(1, { status: 'online', correlationId });
            currentActiveUsers++;
        } else if (currentActiveUsers > 0) {
            activeUsersGauge.add(-1, { status: 'online', correlationId });
            currentActiveUsers--;
        }
    }, 15000);

    const pageLoadDurationHistogram = meter.createHistogram('page_load_duration', {
        description: 'Measures page load durations',
        unit: 'ms',
    });

    setInterval(() => {
        const duration = Math.random() * 5000;
        pageLoadDurationHistogram.record(duration, {
            page: window.location.pathname,
            browser: navigator.userAgent,
            correlationId,
        });
    }, 20000);

    const memoryUsageGauge = meter.createObservableGauge('browser_memory_usage_mb', {
        description: 'Current memory usage of the browser tab in MB',
        unit: 'By',
    });

    memoryUsageGauge.addCallback((observableResult) => {
        if (window.performance && (window.performance as any).memory) {
            const usedJSHeapSizeMB = (window.performance as any).memory.usedJSHeapSize / (1024 * 1024);
            observableResult.observe(usedJSHeapSizeMB, {
                type: 'jsHeap',
                source: 'browser',
                correlationId,
            });
        }
    });

    // --- Logs Setup ---
    const logExporter = new OTLPLogExporter({
        url: `${baseUrl}/v1/logs`,
        headers,
    });

    const loggerProvider = new LoggerProvider({
        resource: detected.merge(custom),
    });
    
    loggerProvider.addLogRecordProcessor(new SimpleLogRecordProcessor(logExporter));
    loggerProvider.addLogRecordProcessor(new SimpleLogRecordProcessor(new ConsoleLogRecordExporter()));

    logs.setGlobalLoggerProvider(loggerProvider);
    const logger = logs.getLogger('react-app-logger');

    setInterval(() => {
        logger.emit({
            severityNumber: SeverityNumber.INFO,
            severityText: 'INFO',
            body: 'User navigated',
            attributes: {
                userId: 'user123',
                path: window.location.pathname,
                correlationId,
            },
        });

        if (Math.random() > 0.7) {
            logger.emit({
                severityNumber: SeverityNumber.WARN,
                severityText: 'WARNING',
                body: 'API call failed.',
                attributes: {
                    endpoint: '/api/data',
                    statusCode: 500,
                    error: 'Network timeout',
                    correlationId,
                },
            });
        }
    }, 7000);
}
