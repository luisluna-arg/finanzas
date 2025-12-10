export interface FetchData<T> {
  items: T[];
}

export async function fetchData<T>(
  url: string,
): Promise<T> {
  const response = await fetch(`${url}`);
  return await response.json();
}
