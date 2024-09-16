import React, { useState, useEffect, useCallback } from 'react';
import { Button as BootstrapButton } from 'react-bootstrap';
import dates from "../../../utils/dates";
import ConfirmationModal from "@/app/components/ui/utils/ConfirmationModal";
import { InputType } from '@/app/components/ui/utils/InputType';
import { Form } from "react-bootstrap";
import moment from 'moment';

// Type Definitions
interface Admin {
    endpoint: string;
    key?: string | string[] | ({
        id: string,
        value: any
    }[]);
    addEnabled?: boolean;
    deleteEnabled?: boolean;
}

export interface Column {
    id?: string;
    key?: string;
    min?: number;
    type?: InputType;
    class?: string;
    label?: string;
    placeholder?: string;
    endpoint?: string;
    header?: {
        classes?: string | string[];
        style?: React.CSSProperties;
    };
    editable?: {
        defaultValue?: any;
    } | boolean;
    conditionalClass?: ConditionalClass[] | ConditionalClass;
    mapper?: {
        id: string;
        label?: string | ((record: any) => string);
    } | Function | any;
    datetime?: {
        timeFormat: "HH:mm",
        timeIntervals: number,
        dateFormat: "DD/MM/YYYY" | "MM/DD/YYYY",
        placeholder: string,
    }
}

export interface ConditionalClass {
    eval: (value: any) => boolean;
    class: string;
}

export interface PaginatedTableProps {
    name: string;
    url: string;
    admin?: Admin;
    rowCount?: number;
    columns: Column[];
    onFetch?: (data: any) => void;
    reloadData?: boolean;
}

interface Data {
    items: any[];
    totalPages: number;
}

const parseUrl = (url: string): {
    queryParams: {
        [k: string]: string;
    },
    baseUrl: string
} => {
    const urlObject = new URL(url);
    const queryParams = Object.fromEntries(urlObject.searchParams.entries());
    const baseUrl = urlObject.origin + urlObject.pathname + urlObject.hash;
    return { queryParams, baseUrl };
}

const objectToUrlParams = (params: Record<string, string | number>) => {
    const urlSearchParams = new URLSearchParams();
    for (const key in params) {
        if (params.hasOwnProperty(key)) {
            urlSearchParams.append(key, params[key].toString());
        }
    }
    return urlSearchParams.toString();
}

const PaginatedTable: React.FC<PaginatedTableProps> = ({
    name,
    url,
    admin,
    rowCount,
    columns,
    onFetch,
    reloadData
}) => {
    const [data, setData] = useState<Data>({ items: [], totalPages: 0 });
    const [totalPages, setTotalPages] = useState<number>(0);
    const [page, setPage] = useState<number>(1);
    const [canPreviousPage, setCanPreviousPage] = useState<boolean>(false);
    const [canNextPage, setCanNextPage] = useState<boolean>(false);
    const [adminAddEnabled] = useState<boolean>(admin?.addEnabled ?? true);
    const [adminDeletedEnabled] = useState<boolean>(admin?.deleteEnabled ?? true);
    const [allSelected, setAllSelected] = useState<boolean>(false);
    const [deleteEnabled, setDeleteEnabled] = useState<boolean>(false);
    const [showDeleteModal, setShowDeleteModal] = useState<boolean>(false);

    const pageSize = rowCount ?? 10;
    const adminRowId = `${name}-edit-row`;

    const updatePage = (pageNumber: number, pageCount?: number) => {
        setPage(pageNumber);
        setCanPreviousPage(pageNumber > 1);
        setCanNextPage(pageNumber < (pageCount ?? totalPages));
    }

    const goToPreviousPage = () => {
        updatePage(page <= 1 ? 1 : page - 1);
    }

    const goToNextPage = () => {
        updatePage(page >= totalPages ? totalPages : page + 1);
    }

    const goToPage = (newPage: number) => {
        updatePage(newPage);
    }

    const fetchData = useCallback(async (fetchUrl: string) => {
        setData({ items: [], totalPages: 0 });
        try {
            if (fetchUrl) {
                const { queryParams, baseUrl } = parseUrl(fetchUrl);
                queryParams["Page"] = page.toString();
                queryParams["PageSize"] = pageSize.toString();
                const params = objectToUrlParams(queryParams);
                const paginatedUrl = `${baseUrl}?${params}`;

                const newData = await (await fetch(paginatedUrl)).json();
                setData(newData);
                setTotalPages(newData.totalPages);
                updatePage(page, newData.totalPages);

                onFetch && onFetch(newData);
            }
        } catch (error) {
            console.error("Error fetching data:", error);
        }
    }, [page, pageSize, onFetch]);

    const getEditRowInputs = () => {
        const row = document.getElementById(adminRowId);
        return [
            ...Array.from(row?.getElementsByTagName("input") ?? []),
            ...Array.from(row?.getElementsByTagName("select") ?? [])
        ];
    };

    const getEditRowValues = () => {
        return getEditRowInputs().reduce<Record<string, any>>((o, i) => {
            const column = columns.find(c => c.id === i.id);
            let columnValue = null;
            if (i && i.type === "checkbox") {
                columnValue = i.checked;
            } else if (column?.type === InputType.DateTime) {
                columnValue = dates.tryGet(i.value);
            } else {
                columnValue = i.value;
            }
            o[i.id] = columnValue;
            return o;
        }, {});
    }

    const getSelectCheckboxes = (isSelected?: boolean) => {
        if (typeof document === "undefined") {
            return [];
        }

        const baseSelector = `table#${name} tr.${name}-data-row td:first-child input[type=checkbox]`;
        const checkboxes: any[] = Array.from(document.querySelectorAll(baseSelector));
        return typeof isSelected === 'undefined' ? checkboxes : checkboxes.filter(checkbox => checkbox.checked === isSelected);
    }

    const validateForm = (inputs?: any[]) => {
        inputs = inputs ?? getEditRowInputs();
        for (const input of inputs) {
            if (
                !input.classList.contains("invisible") &&
                input.type !== "checkbox" &&
                !input.value
            ) {
                break;
            }
        }
    };

    const handleRequest = async (method: string, record: any) => {
        try {
            const headers: HeadersInit = {
                "Content-Type": "application/json",
            };
            if (method === "DELETE") {
                headers["accept"] = "application/octet-stream";
            }

            const response = await fetch(admin?.endpoint ?? '', {
                method: method,
                headers: headers,
                body: JSON.stringify(record),
            });

            if (response.ok) {
                // Handle successful response
            } else {
                // Handle error response
            }
        } catch (error) {
            console.error("Error:", error);
        } finally {
            if (url) fetchData(url);
        }
    };

    const handleCreate = async (inputs: Record<string, any>) => {
        await handleRequest("POST", inputs);
    };

    const handleDelete = async (inputs: { ids: string[] }) => {
        await handleRequest("DELETE", inputs);
    };

    const handleDeleteModalShow = () => setShowDeleteModal(true);

    const handleDeleteModalCancel = () => setShowDeleteModal(false);

    const handleDeleteModalAccept = async () => {
        const ids = getSelectCheckboxes(true).map((i) => i.id);
        await handleDelete({ ids });
        setShowDeleteModal(false);
        fetchData(url);
        setAllSelected(false);
    };

    const onAdd = async () => {
        const values = getEditRowValues();
        validateForm();
        if (admin?.key) {
            const keys: any[] = Array.isArray(admin.key) ? admin.key : [admin.key];
            keys.forEach(key => {
                values[key.id] = key.value;
            });
        }
        await handleCreate(values);
    };

    const onDelete = async (event: React.MouseEvent<HTMLButtonElement>) => {
        await handleDelete({ ids: [event.currentTarget.getAttribute('data-id') ?? ''] });
    };

    useEffect(() => {
        if (url) {
            fetchData(url);
        }
    }, [url, page, pageSize, fetchData, reloadData]);

    const getColumnValue = (columnSettings: Column, record: any) => {
        const columnId: any = columnSettings.id ?? columnSettings.key ?? "";
        const columnValue = record[columnId];

        if (columnSettings.datetime && columnValue) {
            let dateFormat = columnSettings.datetime.dateFormat ?? "";
            let timeFormat = columnSettings.datetime.timeFormat ?? "";
            return moment(columnValue).format(`${dateFormat} ${timeFormat}`);
        }
        else if (columnSettings.mapper) {
            const mapper = columnSettings.mapper;
            if (typeof mapper === 'function') {
                return mapper(columnValue);
            }

            if (mapper.hasOwnProperty("label")) {
                let label = mapper["label"];

                if (typeof label !== 'function') {
                    return columnValue[label!];
                }

                return label(columnValue);
            }
        }
        return columnValue;
    }

    const getEditRowDefaultValue = (columnEditable?: { defaultValue?: any }) => {
        if (columnEditable?.defaultValue === undefined ||
            columnEditable?.defaultValue === null) return "";
        if (typeof columnEditable.defaultValue === "function") {
            return columnEditable.defaultValue();
        }
        return columnEditable.defaultValue;
    };

    const ActionButton: React.FC<{
        text: string;
        action: () => void;
        disabled?: boolean;
        dataId?: string;
        variant?: string;
        width?: string;
        height?: string;
    }> = ({ text, action, disabled, dataId, variant, width, height }) => {
        const buttonStyle: React.CSSProperties = {
            width: width || 'auto',
            height: height || 'auto',
        };

        return (
            <BootstrapButton
                onClick={action}
                disabled={disabled}
                variant={variant}
                style={buttonStyle}
                data-id={dataId}
            >
                {text}
            </BootstrapButton>
        );
    }

    return (
        <div className="paginated-table">
            <Form>
                <table id={name}>
                    <thead>
                        <tr>
                            {adminAddEnabled && <th></th>}
                            {columns.map((column, index) => {
                                let classes = column?.header?.classes;
                                classes = Array.isArray(classes) ? classes.join(" ") : classes;
                                return (
                                    <th key={index} className={classes}>
                                        {column.label}
                                    </th>
                                );
                            })}
                        </tr>
                    </thead>
                    <tbody>
                        {data.items.map((record, index) => (
                            <tr key={index} className={`${name}-data-row`}>
                                {adminAddEnabled && (
                                    <td>
                                        <ActionButton
                                            text="Edit"
                                            action={() => { }}
                                            dataId={record.id}
                                        />
                                    </td>
                                )}
                                {columns.map((column, index) => (
                                    <td key={index} className={column.class}>
                                        {getColumnValue(column, record)}
                                    </td>
                                ))}
                            </tr>
                        ))}
                    </tbody>
                </table>
            </Form>

            {adminAddEnabled && (
                <ActionButton
                    text="Add"
                    action={onAdd}
                />
            )}

            {adminDeletedEnabled && (
                <ActionButton
                    text="Delete"
                    action={handleDeleteModalShow}
                    disabled={!getSelectCheckboxes(true).length}
                />
            )}

            <ConfirmationModal
                show={showDeleteModal}
                handleAccept={handleDeleteModalAccept}
                handleCancel={handleDeleteModalCancel}
                title="Confirm Delete"
                text="Are you sure you want to delete the selected items?"
            />

            <div className="pagination-controls">
                <ActionButton
                    text="Previous"
                    action={goToPreviousPage}
                    disabled={!canPreviousPage}
                />
                <span>
                    Page {page} of {totalPages}
                </span>
                <ActionButton
                    text="Next"
                    action={goToNextPage}
                    disabled={!canNextPage}
                />
            </div>
        </div>
    );
};

export default PaginatedTable;
