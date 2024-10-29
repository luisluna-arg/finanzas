export const parseUrl = (
  url: string
): {
  queryParams: {
    [k: string]: string;
  };
  baseUrl: string;
} => {
  const urlObject = new URL(url);
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
