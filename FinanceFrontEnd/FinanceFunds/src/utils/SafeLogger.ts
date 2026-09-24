/**
 * SafeLogger wraps console calls and gates them to development only.
 * It also redacts common sensitive keys when logging objects.
 */

// Allow console usage here via ESLint override in .eslintrc.cjs
const isDev: boolean = Boolean(import.meta.env.DEV);

function redact(obj: unknown): unknown {
  try {
    if (!obj || typeof obj !== 'object') return obj;

    // Error's own message/stack/name aren't enumerable, so Object.keys(error)
    // is [] and the loop below would silently turn every logged error into
    // an empty {}. Pull them out explicitly first.
    if (obj instanceof Error) {
      return {
        name: obj.name,
        message: obj.message,
        stack: obj.stack,
        ...(obj.cause !== undefined ? { cause: redact(obj.cause) } : {}),
      };
    }

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
  // Errors always surface, in every environment — silencing them in production
  // makes real failures (e.g. an auth callback error) invisible in server logs.
  error: (message?: unknown, ...args: unknown[]) => {
    console.error(message, ...args.map(a => redact(a)));
  },
};

export default SafeLogger;
