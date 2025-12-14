import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  // Enable instrumentation hook to load localStorage polyfill for Node.js v25 compatibility
  experimental: {
    instrumentationHook: true,
  },
};

export default nextConfig;
