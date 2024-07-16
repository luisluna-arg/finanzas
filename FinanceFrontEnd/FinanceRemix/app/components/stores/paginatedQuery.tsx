import { useInfiniteQuery, useQuery } from "react-query";

function usePaginatedQuery(
    queryKey: any,
    fetcher: any,
    options: {
        initialData?: any;
        [key: string]: any; // Allow additional options
    } = {}) {
    const { initialData, ...queryOptions } = options;

    const queryFn: Function = Array.isArray(queryKey) ? useInfiniteQuery : useQuery;

    return queryFn(
        queryKey,
        ({ pageParam = 1, pageSizeParam = 10 }) =>
            fetcher(pageParam, pageSizeParam),
        {
            getPreviousPageParam: (firstPageData: any) => {
                return firstPageData?.page > 1 ? firstPageData.page - 1 : null;
            },
            getNextPageParam: (lastPageData: any) => {
                return lastPageData?.hasMore ? lastPageData.page + 1 : null;
            },
            initialData,
            ...queryOptions,
        }
    );
}

export default usePaginatedQuery;
