// localStorage polyfill for Node.js environment
// This prevents "localStorage.getItem is not a function" errors on Node.js v25

if (typeof window === 'undefined' && typeof global !== 'undefined') {
  // Only apply polyfill in Node.js environment
  if (typeof global.localStorage === 'undefined' || typeof global.localStorage.getItem !== 'function') {
    const storage = new Map();

    global.localStorage = {
      getItem: (key) => storage.get(key) ?? null,
      setItem: (key, value) => storage.set(key, String(value)),
      removeItem: (key) => storage.delete(key),
      clear: () => storage.clear(),
      get length() {
        return storage.size;
      },
      key: (index) => {
        const keys = Array.from(storage.keys());
        return keys[index] ?? null;
      }
    };

    console.log('[Polyfill] localStorage polyfill applied for Node.js environment');
  }
}
