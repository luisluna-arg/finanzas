import React, { useState, useEffect, useCallback } from 'react';
import { Form, FormCheck } from "react-bootstrap";
import moment from 'moment';
import dates from "@/app/utils/dates";
import { InputType } from '@/app/components/ui/utils/InputType';
import ConfirmationModal from "@/app/components/ui/utils/ConfirmationModal";
import ActionButton from '@/app/components/ui/utils/ActionButton';
import Pagination from '@/app/components/ui/utils/Pagination';
import { VARIANTS } from '@/app/components/ui/utils/Bootstrap/ColorVariants';


// TODO: PaginatedTable - Actions still need implementation

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
        dateFormat: "DD/MM/YYYY" | "MM/DD/YYYY" | "DD/MM/yyyy" | "MM/DD/yyyy",
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
    const [adminAddEnabled] = useState<boolean>(admin?.addEnabled ?? true);
    const [adminDeletedEnabled] = useState<boolean>(admin?.deleteEnabled ?? true);
    const [allSelected, setAllSelected] = useState<boolean>(false);
    const [deleteEnabled, setDeleteEnabled] = useState<boolean>(false);
    const [showDeleteModal, setShowDeleteModal] = useState<boolean>(false);

    const pageSize = rowCount ?? 10;
    const adminRowId = `${name}-edit-row`;

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
            console.log(`${dateFormat} ${timeFormat}`);
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

    const handleSelectAllChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setAllSelected(e.target.checked);
    };


    type ActionsRowProps = {
        header?: boolean;
    };

    const ActionsRow = ({ header = false }: ActionsRowProps) => {
        const AddButton = () => (adminAddEnabled &&
            <ActionButton
                text="Agregar"
                classes={["me-2"]}
                action={onAdd}
            />
        );

        const DeleteButton = () => (adminDeletedEnabled &&
            <ActionButton
                text="Eliminar"
                action={handleDeleteModalShow}
                disabled={!getSelectCheckboxes(true).length}
            />
        );

        return <tr>
            {
                header ?
                    (<th colSpan={columnCount}><AddButton /><DeleteButton /></th>) :
                    (<td colSpan={columnCount}><AddButton /><DeleteButton /></td>)
            }
        </tr>;
    }

    const adminEnabled = adminAddEnabled || adminDeletedEnabled;
    const columnCount = columns.length + (adminEnabled ? 2 : 0)

    return (
        <div className="paginated-table ">
            <Form>
                <table id={name} className='table table-striped'>
                    <thead>
                        <ActionsRow header={true} />
                        <tr>
                            {adminEnabled && <th style={{ width: "40px" }}>
                                <FormCheck id={`${name}-select-all`} checked={allSelected} onChange={handleSelectAllChange} />
                            </th>}
                            {columns.map((column, index) => {
                                let classes = column?.header?.classes;
                                classes = Array.isArray(classes) ? classes.join(" ") : classes;
                                return (
                                    <th scope="col" key={index} className={classes}>
                                        {column.label}
                                    </th>
                                );
                            })}
                            {adminEnabled && <th />}
                        </tr>
                    </thead>
                    <tbody>
                        {data.items.map((record, index) => {
                            const rowId = `${name}-data-row-${index}`;
                            return (
                                <tr key={rowId} id={rowId} className={`${name}-data-row`}>
                                    {adminEnabled && <td><FormCheck /></td>}
                                    {columns.map((column, index) => (
                                        <td key={index} className={column.class}>
                                            {getColumnValue(column, record)}
                                        </td>))}
                                    {adminAddEnabled && (
                                        <td style={{ width: "100px" }}>
                                            <ActionButton
                                                text="Editar"
                                                variant={VARIANTS.WARNING}
                                                action={() => { }}
                                                dataId={record.id}
                                            />
                                        </td>
                                    )}
                                </tr>
                            );
                        })}
                    </tbody>
                    <tfoot>
                        <ActionsRow header={true} />
                    </tfoot>
                </table>
            </Form>

            <div className="pagination-controls pagination">
                <Pagination
                    page={page}
                    totalPages={totalPages}
                    action={(newPage: number) => setPage(newPage)}
                />
            </div>

            <ConfirmationModal
                show={showDeleteModal}
                handleAccept={handleDeleteModalAccept}
                handleCancel={handleDeleteModalCancel}
                title="Confirmar eliminación"
                text="¿Estás seguro de que deseas eliminar los elementos seleccionados?"
            />
        </div>
    );
};

export default PaginatedTable;
