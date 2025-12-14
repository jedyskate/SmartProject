import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  // Enable instrumentation for localStorage polyfill
  experimental: {
    instrumentationHook: true,
  },
  webpack: (config, { isServer }) => {
    if (isServer) {
      // Exclude ALL OpenTelemetry packages from server bundle to prevent localStorage issues on Node.js v25
      config.externals = config.externals || [];
      config.externals.push({
        '@opentelemetry/sdk-trace-web': 'commonjs @opentelemetry/sdk-trace-web',
        '@opentelemetry/sdk-trace-base': 'commonjs @opentelemetry/sdk-trace-base',
        '@opentelemetry/instrumentation': 'commonjs @opentelemetry/instrumentation',
        '@opentelemetry/instrumentation-document-load': 'commonjs @opentelemetry/instrumentation-document-load',
        '@opentelemetry/instrumentation-user-interaction': 'commonjs @opentelemetry/instrumentation-user-interaction',
        '@opentelemetry/instrumentation-fetch': 'commonjs @opentelemetry/instrumentation-fetch',
        '@opentelemetry/resources': 'commonjs @opentelemetry/resources',
        '@opentelemetry/sdk-metrics': 'commonjs @opentelemetry/sdk-metrics',
        '@opentelemetry/sdk-logs': 'commonjs @opentelemetry/sdk-logs',
        '@opentelemetry/api': 'commonjs @opentelemetry/api',
        '@opentelemetry/api-logs': 'commonjs @opentelemetry/api-logs',
        '@opentelemetry/exporter-trace-otlp-proto': 'commonjs @opentelemetry/exporter-trace-otlp-proto',
        '@opentelemetry/exporter-metrics-otlp-proto': 'commonjs @opentelemetry/exporter-metrics-otlp-proto',
        '@opentelemetry/exporter-logs-otlp-proto': 'commonjs @opentelemetry/exporter-logs-otlp-proto',
      });
    }
    return config;
  },
};

export default nextConfig;
