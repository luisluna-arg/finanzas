import { useInfiniteQuery, useQuery } from "react-query";

function usePaginatedQuery(queryKey, fetcher, options = {}) {
  const { initialData, ...queryOptions } = options;

  const queryFn = Array.isArray(queryKey) ? useInfiniteQuery : useQuery;

  return queryFn(
    queryKey,
    ({ pageParam = 1, pageSizeParam = 10 }) =>
      fetcher(pageParam, pageSizeParam),
    {
      getPreviousPageParam: (firstPageData) => {
        return firstPageData?.page > 1 ? firstPageData.page - 1 : null;
      },
      getNextPageParam: (lastPageData) => {
        return lastPageData?.hasMore ? lastPageData.page + 1 : null;
      },
      initialData,
      ...queryOptions,
    }
  );
}

export default usePaginatedQuery;
