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

// Initialize Redis client immediately at module load (not lazy)
// React Router v7 only prerenders routes, it doesn't execute server-side code at build time
let redisClient: Redis | MockRedis;

if (process.env.REDIS_URL) {
  console.log("Initializing Redis client with URL:", process.env.REDIS_URL);
  const client = new Redis(process.env.REDIS_URL, {
    maxRetriesPerRequest: null,
    enableReadyCheck: true,
    lazyConnect: false,
    retryStrategy: (times: number) => {
      // Only retry a few times, then give up
      if (times > 3) {
        console.error("Redis max retries exceeded, giving up");
        return null; // Stop retrying
      }
      const delay = Math.min(times * 1000, 3000);
      console.log(`Redis retry attempt ${times}, waiting ${delay}ms`);
      return delay;
    },
  });
  
  client.on("error", (err) => {
    console.error("Redis connection error:", err.message);
  });
  
  client.on("connect", () => {
    console.log("Redis client connected successfully");
  });
  
  client.on("ready", () => {
    console.log("Redis client ready");
  });
  
  client.on("reconnecting", () => {
    console.log("Redis client reconnecting...");
  });
  
  client.on("close", () => {
    console.log("Redis connection closed");
  });
  
  redisClient = client;
} else {
  console.log("No REDIS_URL provided, using in-memory mock");
  redisClient = new MockRedis();
}

// Export the singleton directly - no Proxy, no lazy init
export default redisClient;
