/**
 * Wraps a backend URL. Call .with(params) to attach query params, .append(path) to extend the path.
 * toString() returns the authenticated /api/proxy?path=... URL — pass directly via String().
 */
export class BackendUrl {
  constructor(
    private readonly raw: string,
    private readonly _params?: Record<string, unknown>
  ) {}

  /** Returns a new BackendUrl with additional query parameters merged in. */
  with(params: Record<string, unknown>): BackendUrl {
    return new BackendUrl(this.raw, { ...this._params, ...params });
  }

  /** Returns a new BackendUrl with the given path segment appended. */
  append(path: string): BackendUrl {
    return new BackendUrl(this.raw + path, this._params);
  }

  /**
   * Returns the raw backend URL with params appended as a query string.
   * Use this for server-side HTTP calls (axios, fetch with full URL) that
   * already provide their own Authorization header directly to the backend.
   */
  toRaw(): string {
    if (!this._params) return this.raw;
    const search = Object.entries(this._params)
      .map(([k, v]) => `${encodeURIComponent(k)}=${encodeURIComponent(String(v))}`)
      .join('&');
    const sep = this.raw.includes('?') ? '&' : '?';
    return `${this.raw}${sep}${search}`;
  }

  toString(): string {
    let path = this.raw;
    if (this.raw.startsWith('http')) {
      try {
        const u = new URL(this.raw);
        path = u.pathname + (u.search || '');
      } catch {
        // keep raw as path
      }
    }
    const search = this._params
      ? '&' +
        Object.entries(this._params)
          .map(([k, v]) => `${encodeURIComponent(k)}=${encodeURIComponent(String(v))}`)
          .join('&')
      : '';
    return `/api/proxy?path=${encodeURIComponent(path)}${search}`;
  }
}
