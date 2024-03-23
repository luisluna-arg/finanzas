import React, { useState, useEffect, useCallback } from 'react';
import Button from 'react-bootstrap/Button';
import dates from "../../../utils/dates";
import Input from '../../utils/Input';
import { InputControlTypes } from '../InputControl';

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

const PaginatedTable = ({ name, url, admin, rowCount, columns, onFetch }) => {
    const [data, setData] = useState([]);
    const [totalPages, setTotalPages] = useState(0);
    const [page, setPage] = useState(1);
    const [pageSize, setPageSize] = useState(rowCount ?? 10);
    const [canPreviousPage, setCanPreviousPage] = useState(false);
    const [canNextPage, setCanNextPage] = useState(false);
    const [reload, setReload] = useState(true);
    const [adminAddEnabled,] = useState(admin && (!admin.hasOwnProperty("addEnabled") || admin.addEnabled));
    const [adminDeletedEnabled,] = useState(admin && (!admin.hasOwnProperty("deleteEnabled") || admin.deleteEnabled));

    const adminRowId = `${name}-edit-row`;

    const updatePage = (pageNumber, pageCount) => {
        setPage(pageNumber);
        setCanPreviousPage(pageNumber > 1);
        setCanNextPage(pageNumber < (pageCount ?? totalPages));
        setReload(true);
    }

    const goToPreviousPage = () => {
        updatePage(page <= 1 ? 1 : page - 1);
    }

    const goToNextPage = () => {
        updatePage(page >= totalPages ? totalPages : page + 1);
    }

    const goToPage = (newPage) => {
        updatePage(newPage);
    }

    const fetchData = useCallback(async (fetchUrl) => {
        setData([]);
        try {
            if (reload && fetchUrl) {
                const { queryParams, baseUrl } = parseUrl(fetchUrl);
                queryParams["Page"] = page;
                queryParams["PageSize"] = pageSize;
                const params = objectToUrlParams(queryParams);
                const paginatedUrl = `${baseUrl}?${params}`;

                let newData = await (await fetch(paginatedUrl)).json();
                setData(newData);
                setTotalPages(newData.totalPages);
                updatePage(page, newData.totalPages);

                onFetch && onFetch(newData);
            }
        } catch (error) {
            console.error("Error fetching data:", error);
        } finally {
            setReload(false);
        }
    }, [page]);

    const getEditRowInputs = () => {
        const row = document.getElementById(adminRowId);
        return [
            ...Array.from(row.getElementsByTagName("input")),
            ...Array.from(row.getElementsByTagName("select"))
        ];
    };

    const getEditRowValues = () => {
        return getEditRowInputs().reduce((o, i) => {
            const column = columns.filter(o => o.id === i.id)[0];
            let columnValue = null;
            if (i && i.type === "checkbox") {
                columnValue = i.checked;
            }
            else if (column.type === InputControlTypes.DateTime) {
                columnValue = dates.tryGet(i.value);
            }
            else {
                columnValue = i.value;
            }

            o[i.id] = columnValue;

            return o;
        }, {});
    }

    const validateForm = (inputs) => {
        // setError(null);
        inputs = inputs ?? getEditRowInputs();
        for (let i = 0; i < inputs.length; i++) {
            var input = inputs[i];
            if (
                !input.classList.contains("invisible") &&
                input.type !== "checkbox" &&
                !input.value
            ) {
                // setError("Algunos campos no han sido completados");
                break;
            }
        }
    };

    const handleRequest = async (method, record) => {
        try {
            var headers = {
                "Content-Type": "application/json",
            };

            switch (method) {
                case "DELETE":
                    {
                        headers["accept"] = "application/octet-stream";
                        break;
                    }
                default:
                    break;
            }

            const response = await fetch(admin.endpoint, {
                method: method,
                headers: headers,
                body: JSON.stringify(record),
            });

            if (response.ok) {
                // setRequestStatus(null);
            } else {
                // setRequestStatus("Ocurrió un error en la operación");
            }
        } catch (error) {
            console.error("Error:", error);
            // setRequestStatus("Ocurrió un error en la operación");
        }
        finally {
            fetchData(url);
        }
    };

    const handleCreate = async (inputs) => {
        await handleRequest("POST", inputs);
    };

    const handleDelete = async (inputs) => {
        await handleRequest("DELETE", inputs);
    };

    const onAdd = async (event) => {
        const values = getEditRowValues();
        validateForm();
        if (admin.key) {
            var keys = [];
            if (!Array.isArray(admin.key)) {
                keys.push(admin.key);
            }

            keys.forEach(key => {
                values[key.id] = key.value
            });
        }
        await handleCreate(values);
    };

    const onDelete = async (event) => {
        await handleDelete({ ids: [event.currentTarget.getAttribute('data-id')] });
    };

    useEffect(() => {
        if (url) {
            fetchData(url);
        }
    }, [url, page, pageSize, fetchData]);

    const getColumnValue = (columnSettings, record) => {
        const columnId = columnSettings.id ?? columnSettings.key;

        const mapper = columnSettings.mapper;

        if (mapper) {
            if (typeof mapper == 'function') return mapper(record[columnId]);

            if (mapper.hasOwnProperty("label")) {
                return record[columnSettings.id][mapper["label"]];
            }
        }

        return record[columnId];
    }

    const ActionButton = ({ text, action, disabled, dataId, variant }) => {
        const buttonStyle = {
            width: '2.8em',
            height: "31px"
        };

        return (
            <Button
                variant={variant ?? 'outline-info'}
                size="sm"
                style={buttonStyle}
                className="me-2 small"
                onClick={action}
                disabled={disabled}
                data-id={dataId}>
                {text}
            </Button>
        );
    };

    const EditRow = () => {
        return (<tr id={adminRowId}>
            {columns && columns.map((column, index) => {
                const columnId = column.key ?? column.id;
                if (column.editable) {
                    return (<td className={`${column.class ?? ''} align-middle`} key={`${name}-${columnId}-${index}`}>
                        <Input value={""} settings={column} />
                    </td>);
                }
                else {
                    return (<td className={"align-middle"} key={`${name}-${columnId}-${index}`}></td>);
                }
            })}
            <th className={"align-middle"}>
                <ActionButton text={'+'} action={onAdd} />
            </th>
        </tr>);
    };

    const Navigation = () => {
        return (<>
            <div className='d-flex p-2 justify-content-center'>
                <span className='centered me-2'>
                    Page{' '}
                    <strong>
                        {page} of {totalPages}
                    </strong>
                </span>

                <ActionButton text={'<<'} action={() => goToPage(1)} disabled={!canPreviousPage} />
                <ActionButton text={'<'} action={() => goToPreviousPage()} disabled={!canPreviousPage} />
                <ActionButton text={'>'} action={() => goToNextPage()} disabled={!canNextPage} />
                <ActionButton text={'>>'} action={() => goToPage(totalPages)} disabled={!canNextPage} />
            </div>
        </>);
    }

    return (
        <>
            <Navigation />
            <table id={name} className="table table-sm">
                <thead>
                    <tr>
                        {columns.map((column, index) => {
                            const { classes = "", style = {} } = column.header || {};
                            const classValue = typeof classes === 'string' ? classes : classes.reduce((curr, next) => `${curr} ${next}`);
                            return (
                                <th className={`${classValue} align-bottom`} style={style} scope="col" key={index}>
                                    {column.label}
                                </th>
                            );
                        })}
                        {admin && (<th style={{ width: "60px" }}></th>)}
                    </tr>
                </thead>
                <tbody>
                    {admin && adminAddEnabled && <EditRow />}
                    {(!data || !data.items || data.items.length === 0) &&
                        (<tr>
                            <td colSpan={columns.length + 1} className='align-middle'>
                                <div className='text-center'>No hay datos disponibles</div>
                            </td>
                        </tr>)}
                    {data.items && data.items.map((record) => (
                        <tr key={record.id}>
                            {columns && columns.map((column, index) => {
                                const columnId = column.key ?? column.id;
                                const value = getColumnValue(column, record);
                                const displayValue = column.type && column.type === InputControlTypes.DateTime ? dates.toDisplay(value) : value;
                                const useConditionalClass = column.conditionalClass && column.conditionalClass.eval(record[columnId]);
                                const cssClasses = useConditionalClass ? column.conditionalClass.class : "";

                                return (<td className={`${column.class ?? ''} align-middle`} key={`${name}-${columnId}-${index}`}>
                                    <span className={cssClasses}>{displayValue}</span>
                                </td>);
                            })}
                            {admin && adminDeletedEnabled && (<td className={"align-middle"}>
                                <ActionButton text={'-'} action={onDelete} dataId={record.id} variant={"outline-danger"} />
                            </td>)}
                        </tr>
                    ))}
                </tbody>
            </table>
            <Navigation />
            {/* {isFetching && !isFetchingNextPage && (
                <CustomToast text={"Cargando..."}></CustomToast>
            )}
            {isFetchingNextPage && (
                <CustomToast text={"Cargando más..."}></CustomToast>
            )} */}
        </>
    );
};

export default PaginatedTable;
