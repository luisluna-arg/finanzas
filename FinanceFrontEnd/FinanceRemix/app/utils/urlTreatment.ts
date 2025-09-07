export const parseUrl = (
  url: string
): {
  queryParams: {
    [k: string]: string;
  };
  baseUrl: string;
} => {
  let urlObject: URL;
  try {
    urlObject = new URL(url);
  } catch (e) {
    // If url is relative (starts with /), provide a base using the current origin in the browser
    // or fallback to http://localhost for environments without `window`.
    const base = typeof window !== "undefined" ? window.location.origin : "http://localhost";
    urlObject = new URL(url, base);
  }

  const queryParams = Object.fromEntries(urlObject.searchParams.entries());
  const baseUrl = urlObject.origin + urlObject.pathname + urlObject.hash;
  return { queryParams, baseUrl };
};

export const objectToUrlParams = (params: Record<string, string | number>) => {
  const urlSearchParams = new URLSearchParams();
  for (const key in params) {
    if (params.hasOwnProperty(key)) {
      urlSearchParams.append(key, params[key].toString());
    }
  }
  return urlSearchParams.toString();
};
