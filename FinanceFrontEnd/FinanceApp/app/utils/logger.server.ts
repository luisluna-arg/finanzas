/**
 * Simple server-side logger that only prints in non-production.
 * Use for server loaders/actions where process.env.NODE_ENV is available.
 */
/* eslint-disable no-console */
const isProd = process.env.NODE_ENV === "production";

const logger = {
    info: (...args: unknown[]) => {
        if (!isProd) console.info("[INFO]", ...args);
    },
    debug: (...args: unknown[]) => {
        if (!isProd) console.debug("[DEBUG]", ...args);
    },
    warn: (...args: unknown[]) => {
        if (!isProd) console.warn("[WARN]", ...args);
    },
    error: (...args: unknown[]) => {
        // always print errors so they are visible in server logs
        console.error("[ERROR]", ...args);
    },
};

export default logger;
