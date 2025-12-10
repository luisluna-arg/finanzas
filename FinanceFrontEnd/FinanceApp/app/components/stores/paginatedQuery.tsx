import {
    useInfiniteQuery,
    useQuery,
    UseInfiniteQueryResult,
    UseQueryResult,
    QueryKey,
    InfiniteData,
} from "react-query";

type PageShape = {
    page?: number;
    hasMore?: boolean;
};

function usePaginatedQuery<
    TPage extends PageShape = PageShape,
    TQueryKey extends QueryKey = QueryKey
>(
    queryKey: TQueryKey,
    fetcher: (pageParam?: number, pageSizeParam?: number) => Promise<TPage>,
    options: {
        initialData?: TPage | undefined;
        [key: string]: unknown; // Allow additional options
    } = {}
):
    | UseInfiniteQueryResult<InfiniteData<TPage>, unknown>
    | UseQueryResult<TPage, unknown> {
    const { initialData, ...queryOptions } = options;

    // Capture whether the queryKey was an array on the first render so the
    // hook call order stays stable across re-renders (rules-of-hooks).
    const isArrayKeyRef =
        typeof queryKey !== "undefined"
            ? { current: Array.isArray(queryKey) }
            : { current: false };

    const useInfinite = isArrayKeyRef.current;

    const infiniteInitialData: InfiniteData<TPage> | undefined = initialData
        ? { pages: [initialData], pageParams: [initialData.page ?? 1] }
        : undefined;

    // Call both hooks unconditionally but decide which result to return. To
    // avoid double fetching, provide a stable enabled flag based on useInfinite
    // so only the intended hook is active.
    const infiniteQuery = useInfiniteQuery<
        TPage,
        unknown,
        InfiniteData<TPage>,
        TQueryKey
    >(
        // For infinite queries the key should be an array
        (queryKey as unknown as TQueryKey) ?? ([] as unknown as TQueryKey),
        ({ pageParam = 1 }) => fetcher(pageParam as number, 10),
        {
            getPreviousPageParam: (firstPageData?: TPage | null) => {
                return firstPageData?.page && firstPageData.page > 1
                    ? firstPageData.page - 1
                    : null;
            },
            getNextPageParam: (lastPageData?: TPage | null) => {
                return lastPageData?.hasMore
                    ? (lastPageData.page ?? 0) + 1
                    : null;
            },
            initialData: infiniteInitialData,
            enabled: useInfinite,
            ...queryOptions,
        }
    );

    const normalQuery = useQuery<TPage, unknown>(
        (queryKey as TQueryKey) ?? ("" as unknown as TQueryKey),
        () => fetcher(1, 10),
        {
            initialData,
            enabled: !useInfinite,
            ...queryOptions,
        }
    );

    return (
        useInfinite ? (infiniteQuery as unknown) : (normalQuery as unknown)
    ) as
        | UseInfiniteQueryResult<InfiniteData<TPage>, unknown>
        | UseQueryResult<TPage, unknown>;
}

export default usePaginatedQuery;
