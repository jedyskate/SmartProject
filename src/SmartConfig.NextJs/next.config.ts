import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  // Enable instrumentation hook to load localStorage polyfill for Node.js v25 compatibility
  experimental: {
    instrumentationHook: true,
  },
  // Expose Aspire-injected OTLP configuration to client-side code
  // Aspire automatically sets OTEL_EXPORTER_OTLP_* for server-side,
  // this makes them available in the browser as well
  env: {
    NEXT_PUBLIC_OTEL_EXPORTER_OTLP_ENDPOINT: process.env.OTEL_EXPORTER_OTLP_ENDPOINT || '',
    NEXT_PUBLIC_OTEL_EXPORTER_OTLP_HEADERS: process.env.OTEL_EXPORTER_OTLP_HEADERS || '',
  },
};

export default nextConfig;
