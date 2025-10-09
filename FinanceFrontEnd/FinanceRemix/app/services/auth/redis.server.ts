// Simple in-memory Redis mock for development. When REDIS_URL is set,
// this module will use a real ioredis client instead.

import Redis from "ioredis";

class MockRedis {
  private store = new Map<string, { value: string; expiry?: number }>();

  async get(key: string): Promise<string | null> {
    const item = this.store.get(key);
    if (!item) return null;

    if (item.expiry && item.expiry < Date.now()) {
      this.store.delete(key);
      return null;
    }

    return item.value;
  }

  // Matches common ioredis set signature usage in the repo: set(key, value, 'EX', seconds)
  async set(key: string, value: string, ...args: unknown[]): Promise<void> {
    let expiry: number | undefined;

    // Handle EX (seconds) and PX (milliseconds) options
    if (args.length >= 2 && args[0] === "EX") {
      const seconds = Number(args[1]);
      if (Number.isFinite(seconds)) expiry = Date.now() + seconds * 1000;
    } else if (args.length >= 2 && args[0] === "PX") {
      const ms = Number(args[1]);
      if (Number.isFinite(ms)) expiry = Date.now() + ms;
    }

    this.store.set(key, { value, expiry });
  }

  async del(key: string): Promise<number> {
    const existed = this.store.has(key);
    this.store.delete(key);
    return existed ? 1 : 0;
  }
}

// Export either a real ioredis client when REDIS_URL is provided,
// or the in-memory mock (useful for local dev and unit tests).
let redis: Redis | MockRedis;

if (process.env.REDIS_URL) {
  const client = new Redis(process.env.REDIS_URL);
  client.on("error", (err) => {
    // Log errors but avoid throwing during import
    // Consumers should handle runtime errors when issuing commands
    // (keep this lightweight so imports don't crash the app)
    // eslint-disable-next-line no-console
    console.error("Redis error:", err);
  });
  redis = client;
} else {
  redis = new MockRedis();
}

export default redis;
