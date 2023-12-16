import React, { useState, useEffect, useMemo, Table } from 'react';
import dates from "../../../utils/dates";
import CustomToast from '../CustomToast';

function parseUrl(url) {
    const urlObject = new URL(url);

    const queryParams = Object.fromEntries(urlObject.searchParams.entries());

    const baseUrl = urlObject.origin + urlObject.pathname + urlObject.hash;

    return { queryParams, baseUrl };
}

function objectToUrlParams(params) {
    const urlSearchParams = new URLSearchParams();

    for (const key in params) {
        if (params.hasOwnProperty(key)) {
            urlSearchParams.append(key, params[key]);
        }
    }

    return urlSearchParams.toString();
}

const PaginatedTable = ({ name, url, columns, onFetch }) => {
    const [data, setData] = useState([]);
    const [totalPages, setTotalPages] = useState(0);
    const [page, setPage] = useState(1);
    const [canPreviousPage, setCanPreviousPage] = useState(false);
    const [canNextPage, setCanNextPage] = useState(false);
    const [pageSize, setPageSize] = useState(10);
    const [isFetching, setIsFetching] = useState(false);
    const [isFetchingNextPage, setIsFetchingNextPage] = useState(false);

    const goToPreviousPage = () => {
        const newPage = page <= 1 ? 1 : page - 1;
        setPage(newPage);
        setCanPreviousPage(newPage > 0);
    }
    
    const goToNextPage = () => {
        const newPage = page >= totalPages - 1 ? totalPages : page + 1;
        setPage(newPage);
        setCanNextPage(newPage < totalPages);
    }

    const goToPage = (newPage) => {
        setPage(newPage);
        setCanPreviousPage(newPage > 0);
        setCanNextPage(newPage < totalPages);
    }

    const fetchData = async (dataUrl) => {
        setData([]);
        try {
            setIsFetching(true);
            if (dataUrl) {
                console.log(dataUrl);

                let fetchData = await fetch(dataUrl);
                let newData = await fetchData.json();
                setData(newData);
                setTotalPages(newData.totalPages);
                setCanPreviousPage(newData.page > 1);
                setCanNextPage(newData.totalPages > newData.page);

                console.log("data", newData);

                onFetch && onFetch(newData);
            }
        } catch (error) {
            console.error("Error fetching data:", error);
        } finally {
            setIsFetching(false);
        }
    };

    useEffect(() => {
        if (url) {
            const { queryParams, baseUrl } = parseUrl(url);
            queryParams["Page"] = page;
            queryParams["PageSize"] = pageSize;
            const params = objectToUrlParams(queryParams);
            const fetchUrl = `${baseUrl}?${params}`;
            fetchData(fetchUrl);
        }
    }, [url, page, pageSize]);

    if (!data || !data.items || data.items.length == 0) return (<div className='text-center'>No hay datos disponibles</div>);

    return (
        <>
            <table id={name} className="table">
                <thead>
                    <tr>
                        {columns.map((column, index) => (
                            <th className={column.headerClass} scope="col" key={index}>
                                {column.label}
                            </th>
                        ))}
                    </tr>
                </thead>
                <tbody>
                    {data.items && data.items.map((record) => (
                        <tr key={record.id}>
                            {columns && columns.map((column, index) => {
                                const value = column.mapper ? column.mapper(record[column.id]) : record[column.id];
                                const displayValue = column.type && column.type == "datetime" ? dates.toDisplay(value) : value;
                                const useConditionalClass = column.conditionalClass && column.conditionalClass.eval(record[column.id]);
                                const cssClasses = useConditionalClass ? column.conditionalClass.class : "";

                                return (<td className={column.class} key={`${name}-${column.id}-${index}`}>
                                    <span className={cssClasses}>{displayValue}</span>
                                </td>);
                            })}
                        </tr>
                    ))}
                </tbody>
            </table>
            {isFetching && !isFetchingNextPage && (
                <CustomToast text={"Cargando..."}></CustomToast>
            )}
            {isFetchingNextPage && (
                <CustomToast text={"Cargando más..."}></CustomToast>
            )}
            <div className='d-flex p-2 justify-content-center'>
                <span className='d-inline p-2 me-2'>
                    Page{' '}
                    <strong>
                        {page} of {totalPages}
                    </strong>
                </span>
                <button className='btn btn-outline-primary me-2' onClick={() => goToPage(1)} disabled={!canPreviousPage}>
                    {'<<'}
                </button>
                <button className='btn btn-outline-primary me-2' onClick={() => goToPreviousPage()} disabled={!canPreviousPage}>
                    {'<'}
                </button>
                <button className='btn btn-outline-primary me-2' onClick={() => goToNextPage()} disabled={!canNextPage}>
                    {'>'}
                </button>
                <button className='btn btn-outline-primary' onClick={() => goToPage(totalPages)} disabled={!canNextPage}>
                    {'>>'}
                </button>
            </div>
        </>
    );
};

export default PaginatedTable;
