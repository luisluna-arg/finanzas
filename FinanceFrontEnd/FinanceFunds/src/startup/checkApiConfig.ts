export function getApiConfigError(): string | null {
  const raw = import.meta.env.VITE_API_URL;

  if (!raw) {
    if (import.meta.env.DEV) return null; // allow dev fallback elsewhere
    return 'Missing required environment variable VITE_API_URL. Please set the API base URL for this environment.';
  }

  try {
    const parsed = new URL(raw);
    if (!import.meta.env.DEV && parsed.protocol !== 'https:') {
      return `Insecure API URL protocol '${parsed.protocol}'. Production requires HTTPS.`;
    }
  } catch (err) {
    return `Invalid VITE_API_URL: ${String(err)}`;
  }

  return null;
}
