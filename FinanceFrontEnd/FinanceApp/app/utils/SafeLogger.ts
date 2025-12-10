/**
 * Lightweight SafeLogger for server-side code under the Remix app.
 * Mirrors client SafeLogger interface but uses process.env.NODE_ENV.
 */
/* eslint-disable no-console */
const isDev = process.env.NODE_ENV !== "production";

function redact(obj: unknown) {
    if (!obj || typeof obj !== "object") return obj;
    try {
        const clone: Record<string, unknown> | unknown[] = Array.isArray(obj)
            ? []
            : {};
        for (const k of Object.keys(obj as Record<string, unknown>)) {
            const lower = k.toLowerCase();
            if (
                lower.includes("secret") ||
                lower.includes("token") ||
                lower.includes("password") ||
                lower.includes("access")
            ) {
                (clone as Record<string, unknown>)[k] = "[REDACTED]";
            } else {
                (clone as Record<string, unknown>)[k] = (
                    obj as Record<string, unknown>
                )[k];
            }
        }
        return clone;
    } catch {
        return "[UNREDACTABLE]";
    }
}

const SafeLogger = {
    log: (message?: unknown, ...args: unknown[]) => {
        if (!isDev) return;
        // Use warn to satisfy lint rules that disallow console.log; SafeLogger is dev-only
        console.warn(message, ...args.map(redact));
    },
    info: (message?: unknown, ...args: unknown[]) => {
        if (!isDev) return;
        console.warn(message, ...args.map(redact));
    },
    warn: (message?: unknown, ...args: unknown[]) => {
        if (!isDev) return;
        console.warn(message, ...args.map(redact));
    },
    error: (message?: unknown, ...args: unknown[]) => {
        // Always print errors so they are visible in server logs
        console.error(message, ...args.map(redact));
    },
};

export default SafeLogger;
