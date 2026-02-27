import { fetchData } from '@/components/data/fetchData';
import { BackendUrl } from '@/utils/BackendUrl';

export interface PaginationData<T> {
  items: T[];
  totalItems: number;
  totalPages: number;
  currentPage: number;
}

export async function fetchPaginatedData<T>(
  url: BackendUrl | string,
  page: number,
  pageSize: number = 10
): Promise<PaginationData<T>> {
  const backendUrl = url instanceof BackendUrl ? url : new BackendUrl(String(url));
  const paginatedUrl = backendUrl.with({ Page: page, PageSize: pageSize });

  const result = await fetchData<PaginationData<T>>(paginatedUrl);
  return {
    items: result.items,
    totalItems: result.totalItems,
    totalPages: result.totalPages,
    currentPage: result.currentPage,
  };
}
