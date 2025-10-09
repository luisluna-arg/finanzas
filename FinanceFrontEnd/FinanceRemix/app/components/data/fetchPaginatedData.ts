import { objectToUrlParams, parseUrl } from "@/utils/urlTreatment";
import { fetchData } from "@/components/data/fetchData";

export interface PaginationData<T> {
    items: T[];
    totalItems: number;
    totalPages: number;
    currentPage: number;
}

export async function fetchPaginatedData<T>(
    url: string,
    page: number,
    pageSize: number = 10
): Promise<PaginationData<T>> {
    const { queryParams, baseUrl } = parseUrl(url);
    queryParams["Page"] = page.toString();
    queryParams["PageSize"] = pageSize.toString();
    const params = objectToUrlParams(queryParams);
    const paginatedUrl = `${baseUrl}?${params}`;

    const result = await fetchData<PaginationData<T>>(`${paginatedUrl}`);
    return {
        items: result.items,
        totalItems: result.totalItems,
        totalPages: result.totalPages,
        currentPage: result.currentPage,
    };
}
