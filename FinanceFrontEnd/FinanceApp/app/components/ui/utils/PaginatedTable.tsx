/* eslint-disable react/prop-types */
/* eslint-disable react/prop-types */
import React, { useState, useEffect } from "react";
import type { Settings as InputControlSettings } from "@/components/ui/utils/InputControl";
import { Form } from "@/components/ui/utils/Form";
import dates from "@/utils/dates";
import ActionButton, {
    ACTION_BUTTON_KIND,
    VARIANTS,
} from "@/components/ui/utils/ActionButton";
import ConfirmationModal from "@/components/ui/utils/ConfirmationModal";
import Input from "@/components/ui/utils/Input";
import LoadingSpinner from "@/components/ui/utils/LoadingSpinner";
import PaginationBar from "@/components/ui/utils/PaginationBar";
import { InputType } from "@/components/ui/utils/InputType";
import { fetchData } from "@/components/data/fetchData";
import { fetchPaginatedData } from "@/components/data/fetchPaginatedData";
import { handleRequest } from "@/components/data/handleRequest";
import {
    Table,
    TableBody,
    TableHeader,
    TableFooter,
    TableRow,
    TableCell,
    TableHead,
} from "@/components/ui/shadcn/table";
import { cn } from "@/lib/utils";
import { Checkbox } from "@/components/ui/shadcn/checkbox";
import { CheckedState } from "@radix-ui/react-checkbox";

// TODO: PaginatedTable - Actions still need implementation

// Type Definitions
interface Admin {
    endpoint: string;
    key?:
        | string
        | string[]
        | {
              id: string;
              value: unknown;
          }[];
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
    editable?:
        | {
              defaultValue?: unknown;
          }
        | boolean;
    conditionalClass?: ConditionalClass[] | ConditionalClass;
    mapper?:
        | {
              id: string;
              label?: string | ((record: Row) => string);
          }
        | ((record: Row) => React.ReactNode);
    datetime?: {
        timeFormat: "HH:mm";
        timeIntervals: number;
        dateFormat: "DD/MM/YYYY" | "MM/DD/YYYY" | "DD/MM/yyyy" | "MM/DD/yyyy";
        placeholder: string;
    };
}

export interface ConditionalClass {
    eval: (value: unknown) => boolean;
    class: string;
}

export interface PaginatedTableProps {
    name: string;
    data?: Data | null;
    url?: string;
    admin?: Admin;
    rowCount?: number;
    columns: Column[];
    onFetch?: (data: unknown) => void;
    onAdd?: (data: unknown) => void;
    onDelete?: (data: unknown) => void;
    reloadData?: boolean;
}

interface Row {
    id?: string | number;
    isSelected?: boolean;
    [key: string]: unknown;
}

interface Data {
    items: Row[];
    totalPages: number;
}

const PaginatedTable: React.FC<PaginatedTableProps> = ({
    name,
    data,
    url,
    admin,
    rowCount,
    columns,
    onAdd,
    onDelete,
    // onFetch and reloadData intentionally unused in this component
}) => {
    const [tableData, setTableData] = useState<Data>({
        items: [],
        totalPages: 0,
    });
    const [totalPages, setTotalPages] = useState<number>(0);
    const [page, setPage] = useState<number>(1);
    const [adminAddEnabled] = useState<boolean>(admin?.addEnabled ?? true);
    const [adminDeletedEnabled] = useState<boolean>(
        admin?.deleteEnabled ?? true
    );
    const [anySelected, setAnySelected] = useState<boolean>(false);
    const [allSelected, setAllSelected] = useState<boolean>(false);
    const [showDeleteModal, setShowDeleteModal] = useState<boolean>(false);
    const [loading, setLoading] = useState(!url ? false : true);
    const [reloadFlag, setReloadFlag] = useState(!url ? false : true);

    const pageSize = rowCount ?? 10;
    const adminRowId = `${name}-edit-row`;

    useEffect(() => {
        if (url) {
            setLoading(true);

            fetchPaginatedData<Data>(url, page, 10).then((result) => {
                const extendedItems = result.items.map((item) => ({
                    ...item,
                    isSelected: false,
                }));

                setTableData({
                    ...result,
                    items: extendedItems,
                });
                setTotalPages(result.totalPages);

                setLoading(false);
                setReloadFlag(false);
            });
        }
    }, [url, page, pageSize, reloadFlag]);

    const getEditRowInputs = () => {
        const row = document.getElementById(adminRowId);
        return [
            ...Array.from(row?.getElementsByTagName("input") ?? []),
            ...Array.from(row?.getElementsByTagName("select") ?? []),
        ] as Array<HTMLInputElement | HTMLSelectElement>;
    };

    const getEditRowValues = () => {
        return getEditRowInputs().reduce<Record<string, unknown>>((o, i) => {
            const column = columns.find((c) => c.id === i.id);
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
    };

    const getSelectCheckboxes = (isSelected?: boolean) => {
        if (typeof document === "undefined") {
            return [];
        }

        const baseSelector = `Table#${name} TableRow.${name}-data-row TableCell:first-child input[type=checkbox]`;
        const nodeList = document.querySelectorAll(baseSelector);
        const checkboxes = Array.from(nodeList).filter(
            (n): n is HTMLInputElement => n instanceof HTMLInputElement
        );
        return typeof isSelected === "undefined"
            ? checkboxes
            : checkboxes.filter((checkbox) => checkbox.checked === isSelected);
    };

    const getSelectIds = (isSelected?: boolean) =>
        getSelectCheckboxes(isSelected).map((i) => i.id);

    const validateForm = (
        inputs?: Array<HTMLInputElement | HTMLSelectElement>
    ) => {
        const resolved = inputs ?? getEditRowInputs();
        if (!resolved) return;
        for (const input of resolved) {
            if (
                !input.classList.contains("invisible") &&
                input.type !== "checkbox" &&
                !input.value
            ) {
                break;
            }
        }
    };

    const handleCreate = async (inputs: Record<string, unknown>) => {
        await handleRequest(admin?.endpoint ?? "", "POST", inputs, false);
        setReloadFlag(true);
        if (onAdd) {
            onAdd(inputs);
        }
    };

    const handleDelete = async (inputs: { ids: string[] }) => {
        await handleRequest(admin?.endpoint ?? "", "DELETE", inputs);
        setReloadFlag(true);
        if (onDelete) {
            onDelete(inputs);
        }
    };

    const handleDeleteModalShow = () => setShowDeleteModal(true);

    const handleDeleteModalCancel = () => setShowDeleteModal(false);

    const handleDeleteModalAccept = async () => {
        const ids = getSelectIds(true);
        await handleDelete({ ids });
        setShowDeleteModal(false);
        if (url) {
            fetchData(url);
        }
        setAllSelected(false);
    };

    const onAddAction = async () => {
        const values = getEditRowValues();
        validateForm();
        if (admin?.key) {
            const keys = Array.isArray(admin.key)
                ? (admin.key as { id: string; value: unknown }[])
                : [{ id: String(admin.key), value: undefined }];
            keys.forEach((key) => {
                (values as Record<string, unknown>)[key.id] = key.value;
            });
        }
        await handleCreate(values);
    };

    const ColumnValue = ({
        columnSettings,
        record,
    }: {
        columnSettings: Column;
        record: Row;
    }) => {
        const columnId = String(columnSettings.id ?? columnSettings.key ?? "");
        let columnValue: unknown = (record as Record<string, unknown>)[
            columnId
        ];

        if (columnSettings.datetime && columnValue) {
            columnValue = dates.toDisplay(String(columnValue));
        } else if (columnSettings.mapper) {
            const mapper = columnSettings.mapper;
            if (typeof mapper === "function") {
                columnValue = mapper(record);
            } else if (mapper && Object.hasOwn(mapper, "label")) {
                const label = (
                    mapper as { label?: string | ((r: Row) => string) }
                ).label;

                if (typeof label !== "function") {
                    columnValue = (record as Record<string, unknown>)[
                        String(label)
                    ];
                } else {
                    columnValue = label(record);
                }
            }
        }

        if (columnSettings.type === InputType.Boolean) {
            return (
                <Checkbox
                    id={String(record.id)}
                    checked={Boolean(columnValue)}
                />
            );
        }

        return <>{columnValue}</>;
    };

    const getEditRowDefaultValue = (columnEditable?: {
        defaultValue?: unknown;
    }) => {
        if (
            columnEditable?.defaultValue === undefined ||
            columnEditable?.defaultValue === null
        )
            return "";
        if (typeof columnEditable.defaultValue === "function") {
            return (columnEditable.defaultValue as () => unknown)();
        }
        return columnEditable.defaultValue;
    };

    const handleSelectAllChange = (e: CheckedState) => {
        const isSelected = e === true;
        setAllSelected(isSelected);
        const updatedItems = tableData.items.map((item) => ({
            ...item,
            isSelected: isSelected,
        }));
        setTableData((prevData) => ({
            ...prevData,
            items: updatedItems,
        }));
        setAnySelected(updatedItems.some((item) => item.isSelected));
    };

    const handleRowCheckChange = (id: string | number) => {
        const updatedItems = tableData.items.map((item) =>
            item.id === id ? { ...item, isSelected: !item.isSelected } : item
        );

        setTableData((prevData) => ({
            ...prevData,
            items: updatedItems,
        }));

        // Update selectAll based on individual checkbox states
        setAnySelected(updatedItems.some((item) => item.isSelected));
        setAllSelected(updatedItems.every((item) => item.isSelected));
    };

    type ActionsRowProps = {
        header?: boolean;
    };

    const ActionsRow = ({ header = false }: ActionsRowProps) => {
        const AddButton = () =>
            adminAddEnabled && (
                <ActionButton
                    type={ACTION_BUTTON_KIND.add}
                    className={["me-2"]}
                    onClick={onAddAction}
                />
            );

        const DeleteButton = () => {
            return (
                adminDeletedEnabled && (
                    <ActionButton
                        type={ACTION_BUTTON_KIND.delete}
                        variant={VARIANTS.destructive}
                        onClick={handleDeleteModalShow}
                        disabled={!anySelected}
                    />
                )
            );
        };

        return (
            <TableRow>
                {header ? (
                    <TableHead className={"w-[100px]"} colSpan={columnCount}>
                        <AddButton />
                        <DeleteButton />
                    </TableHead>
                ) : (
                    <TableCell colSpan={columnCount}>
                        <AddButton />
                        <DeleteButton />
                    </TableCell>
                )}
            </TableRow>
        );
    };

    const EditRow = () => {
        return (
            <TableRow id={adminRowId} className={cn(`${name}-edit-row`)}>
                <TableCell></TableCell>
                {columns &&
                    columns.map((column: Column, index: number) => {
                        const columnId = column.key ?? column.id;
                        if (column.editable) {
                            return (
                                <TableCell
                                    className={cn(column.class, "centered")}
                                    key={`${name}-${columnId}-${index}`}
                                >
                                    <Input
                                        value={String(
                                            getEditRowDefaultValue(
                                                typeof column.editable ===
                                                    "object"
                                                    ? column.editable
                                                    : {
                                                          defaultValue:
                                                              undefined,
                                                      }
                                            ) ?? ""
                                        )}
                                        settings={
                                            column as unknown as InputControlSettings
                                        }
                                    />
                                </TableCell>
                            );
                        } else {
                            return (
                                <TableCell
                                    className={"centered"}
                                    key={`${name}-${columnId}-${index}`}
                                ></TableCell>
                            );
                        }
                    })}
                <TableHead className={cn(["w-[100px]", "text-center"])}>
                    <ActionButton
                        type={ACTION_BUTTON_KIND.add}
                        onClick={onAddAction}
                    />
                </TableHead>
            </TableRow>
        );
    };

    const LoadingTableBody = ({
        columnsLength,
        adminEnabled,
        adminAddEnabled,
    }: {
        columnsLength: number;
        adminEnabled: boolean;
        adminAddEnabled: boolean;
    }) => {
        return (
            <TableBody>
                <TableRow>
                    <TableCell
                        colSpan={
                            columnsLength +
                            (adminEnabled ? 1 : 0) +
                            (adminAddEnabled ? 1 : 0)
                        }
                    >
                        <center>
                            <LoadingSpinner />
                        </center>
                    </TableCell>
                </TableRow>
            </TableBody>
        );
    };

    const ContentTableBody = ({
        tableData,
        admin,
        adminAddEnabled,
    }: {
        tableData: Data;
        admin: Admin | undefined;
        adminAddEnabled: boolean;
    }) => {
        return (
            <TableBody>
                {admin && adminAddEnabled && <EditRow />}
                {tableData.items.map((record: Row, index: number) => {
                    const rowId = `${name}-data-row-${index}`;
                    return (
                        <TableRow
                            key={rowId}
                            id={rowId}
                            className={`${name}-data-row`}
                        >
                            {adminEnabled && (
                                <TableCell>
                                    <Checkbox
                                        id={String(record.id)}
                                        checked={Boolean(record.isSelected)}
                                        onChange={() =>
                                            handleRowCheckChange(
                                                record.id ?? index
                                            )
                                        }
                                    />
                                </TableCell>
                            )}
                            {columns.map((column: Column, index: number) => (
                                <TableCell key={index} className={column.class}>
                                    <ColumnValue
                                        columnSettings={column}
                                        record={record}
                                    />
                                </TableCell>
                            ))}
                            {adminAddEnabled && (
                                <TableCell
                                    style={{
                                        width: "100px",
                                        textAlign: "center",
                                    }}
                                >
                                    <ActionButton
                                        type={ACTION_BUTTON_KIND.delete}
                                        variant={VARIANTS.secondary}
                                        onClick={() =>
                                            handleDelete({
                                                ids: [String(record.id)],
                                            })
                                        }
                                    />
                                </TableCell>
                            )}
                        </TableRow>
                    );
                })}
            </TableBody>
        );
    };

    const adminEnabled = adminAddEnabled || adminDeletedEnabled;
    const columnCount = columns.length + (adminEnabled ? 2 : 0);

    return (
        <div className="paginated-Table">
            <Form>
                <Table id={name}>
                    <TableHeader>
                        <ActionsRow header={true} />
                        <TableRow>
                            {adminEnabled && (
                                <TableHead
                                    className={"w-[100px]"}
                                    style={{ width: "40px" }}
                                >
                                    <Checkbox
                                        id={`${name}-select-all`}
                                        checked={allSelected}
                                        onCheckedChange={handleSelectAllChange}
                                    />
                                </TableHead>
                            )}
                            {columns.map((column, index) => {
                                return (
                                    <TableHead
                                        key={index}
                                        className={cn(column?.header?.classes)}
                                        style={column.header?.style}
                                    >
                                        {column.label}
                                    </TableHead>
                                );
                            })}
                            {adminEnabled && (
                                <TableHead className={"w-[100px]"} />
                            )}
                        </TableRow>
                    </TableHeader>
                    {loading ? (
                        <LoadingTableBody
                            columnsLength={columns.length}
                            adminEnabled={adminEnabled}
                            adminAddEnabled={adminAddEnabled}
                        />
                    ) : (
                        <ContentTableBody
                            admin={admin}
                            adminAddEnabled={adminAddEnabled}
                            tableData={data ?? tableData}
                        />
                    )}
                    <TableFooter>
                        <ActionsRow header={true} />
                    </TableFooter>
                </Table>
            </Form>

            <div className="pagination-controls pagination d-flex justify-content-center">
                <PaginationBar
                    page={page}
                    totalPages={totalPages}
                    action={(newPage: number) => {
                        setPage(newPage);
                    }}
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
