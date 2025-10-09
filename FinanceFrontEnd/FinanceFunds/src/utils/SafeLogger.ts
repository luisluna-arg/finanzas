/**
 * SafeLogger wraps console calls and gates them to development only.
 * It also redacts common sensitive keys when logging objects.
 */

// Allow console usage here via ESLint override in .eslintrc.cjs
const isDev: boolean = Boolean(import.meta.env.DEV);

function redact(obj: unknown): unknown {
  try {
    if (!obj || typeof obj !== 'object') return obj;
    const asRecord = obj as Record<string, unknown>;
    const clone: Record<string, unknown> | unknown[] = Array.isArray(asRecord) ? [] : {};
    for (const k of Object.keys(asRecord)) {
      const lower = k.toLowerCase();
      if (
        lower.includes('secret') ||
        lower.includes('token') ||
        lower.includes('password') ||
        lower.includes('access')
      ) {
        (clone as Record<string, unknown>)[k] = '[REDACTED]';
      } else {
        (clone as Record<string, unknown>)[k] = asRecord[k];
      }
    }
    return clone;
  } catch {
    return '[UNREDACTABLE]';
  }
}

const SafeLogger = {
  log: (message?: unknown, ...args: unknown[]) => {
    if (!isDev) return;
    // Use warn to satisfy project lint rules (allow only warn/error outside of tests)
    console.warn(message, ...args.map(a => redact(a)));
  },
  info: (message?: unknown, ...args: unknown[]) => {
    if (!isDev) return;
    // Info mapped to warn to conform with no-console policy
    console.warn(message, ...args.map(a => redact(a)));
  },
  warn: (message?: unknown, ...args: unknown[]) => {
    if (!isDev) return;
    console.warn(message, ...args.map(a => redact(a)));
  },
  error: (message?: unknown, ...args: unknown[]) => {
    if (!isDev) return;
    console.error(message, ...args.map(a => redact(a)));
  },
};

export default SafeLogger;
