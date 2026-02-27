import type { BackendUrl } from '@/utils/BackendUrl';

export interface FetchData<T> {
  items: T[];
}

export async function fetchData<T>(url: BackendUrl | string): Promise<T> {
  const response = await fetch(String(url));
  return await response.json();
}
