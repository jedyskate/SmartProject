// This file is automatically loaded by Next.js during server initialization
// It runs before any other server-side code, making it perfect for polyfills

export async function register() {
  if (process.env.NEXT_RUNTIME === 'nodejs') {
    // Load localStorage polyfill for Node.js environment
    await import('./lib/polyfills/localStorage.js');
  }
}
