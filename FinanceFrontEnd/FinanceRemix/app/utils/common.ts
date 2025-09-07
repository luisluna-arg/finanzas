const CommonUtils = {
  Params: (obj: Record<string, any>): string =>
    Object.entries(obj)
      .map(
        ([key, value]) =>
          `${encodeURIComponent(key)}=${encodeURIComponent(value)}`
      )
      .join("&"),
};

export default CommonUtils;

export interface Dictionary<T> {
  [key: string]: T;
}

export type ValueLike = number | { value?: number } | null | undefined;

export const toNumber = (r: ValueLike, def = 0): number => {
  if (r === null || r === undefined) return def;
  if (typeof r === "number") return r;
  const v = (r as { value?: unknown }).value;
  return typeof v === "number" ? v : def;
};
