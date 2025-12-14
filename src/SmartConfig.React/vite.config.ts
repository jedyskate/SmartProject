import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import fs from 'fs';
import path from 'path';

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    https: {
      key: fs.readFileSync(path.resolve(__dirname, 'cert/key.pem')),
      cert: fs.readFileSync(path.resolve(__dirname, 'cert/cert.pem')),
    },
    port: 5175, // Optional: use any free port
  },
  // Expose Aspire-injected OTLP configuration to client-side code
  // Aspire automatically sets OTEL_EXPORTER_OTLP_* for server-side,
  // this makes them available in the browser with VITE_ prefix
  define: {
    'import.meta.env.VITE_OTEL_EXPORTER_OTLP_ENDPOINT': JSON.stringify(process.env.OTEL_EXPORTER_OTLP_ENDPOINT || ''),
    'import.meta.env.VITE_OTEL_EXPORTER_OTLP_HEADERS': JSON.stringify(process.env.OTEL_EXPORTER_OTLP_HEADERS || ''),
  },
})
