import type { Config } from "@react-router/dev/config";

export default {
  // Ensure server-only modules are not bundled into client code
  serverModuleFormat: "esm",
  
  // Ensure .server files are treated as server-only
  // This prevents Redis and other server-only code from being included in client bundles
} satisfies Config;
