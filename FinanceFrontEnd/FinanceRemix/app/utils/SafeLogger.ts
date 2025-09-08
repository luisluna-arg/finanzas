/**
 * Lightweight SafeLogger for server-side code under the Remix app.
 * Mirrors client SafeLogger interface but uses process.env.NODE_ENV.
 */
/* eslint-disable no-console */
const isDev = process.env.NODE_ENV !== 'production';

function redact(obj: any) {
  if (!obj || typeof obj !== 'object') return obj;
  try {
    const clone: any = Array.isArray(obj) ? [] : {};
    for (const k of Object.keys(obj)) {
      const lower = k.toLowerCase();
      if (lower.includes('secret') || lower.includes('token') || lower.includes('password') || lower.includes('access')) {
        clone[k] = '[REDACTED]';
      } else {
        clone[k] = obj[k];
      }
    }
    return clone;
  } catch {
    return '[UNREDACTABLE]';
  }
}

const SafeLogger = {
  log: (message?: any, ...args: any[]) => {
    if (!isDev) return;
    console.log(message, ...args.map(redact));
  },
  info: (message?: any, ...args: any[]) => {
    if (!isDev) return;
    console.info(message, ...args.map(redact));
  },
  warn: (message?: any, ...args: any[]) => {
    if (!isDev) return;
    console.warn(message, ...args.map(redact));
  },
  error: (message?: any, ...args: any[]) => {
    if (!isDev) return;
    console.error(message, ...args.map(redact));
  },
};

export default SafeLogger;
