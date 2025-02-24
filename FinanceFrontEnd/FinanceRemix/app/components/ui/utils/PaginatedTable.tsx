import React, { useState, useEffect } from "react";
import { Form } from "@/components/ui/utils/Form";
import moment from "moment";

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
import { Checkbox } from "../shadcn/checkbox";
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
        value: any;
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
        defaultValue?: any;
      }
    | boolean;
  conditionalClass?: ConditionalClass[] | ConditionalClass;
  mapper?:
    | {
        id: string;
        label?: string | ((record: any) => string);
      }
    | Function
    | any;
  datetime?: {
    timeFormat: "HH:mm";
    timeIntervals: number;
    dateFormat: "DD/MM/YYYY" | "MM/DD/YYYY" | "DD/MM/yyyy" | "MM/DD/yyyy";
    placeholder: string;
  };
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

const PaginatedTable: React.FC<PaginatedTableProps> = ({
  name,
  url,
  admin,
  rowCount,
  columns,
  onFetch,
  reloadData,
}) => {
  const [data, setData] = useState<Data>({ items: [], totalPages: 0 });
  const [totalPages, setTotalPages] = useState<number>(0);
  const [page, setPage] = useState<number>(1);
  const [adminAddEnabled] = useState<boolean>(admin?.addEnabled ?? true);
  const [adminDeletedEnabled] = useState<boolean>(admin?.deleteEnabled ?? true);
  const [anySelected, setAnySelected] = useState<boolean>(false);
  const [allSelected, setAllSelected] = useState<boolean>(false);
  const [showDeleteModal, setShowDeleteModal] = useState<boolean>(false);
  const [loading, setLoading] = useState(true);
  const [reloadFlag, setReloadFlag] = useState(true);

  const pageSize = rowCount ?? 10;
  const adminRowId = `${name}-edit-row`;

  useEffect(() => {
    setLoading(true);

    fetchPaginatedData<any>(url, page, 10).then((data) => {
      const extendedItems = data.items.map((item: Omit<any, "isSelected">) => ({
        ...item,
        isSelected: false,
      }));

      setData({
        ...data,
        items: extendedItems,
      });
      setTotalPages(data.totalPages);

      setLoading(false);
      setReloadFlag(false);
    });
  }, [url, page, pageSize, reloadFlag]);

  const getEditRowInputs = () => {
    const row = document.getElementById(adminRowId);
    return [
      ...Array.from(row?.getElementsByTagName("input") ?? []),
      ...Array.from(row?.getElementsByTagName("select") ?? []),
    ];
  };

  const getEditRowValues = () => {
    return getEditRowInputs().reduce<Record<string, any>>((o, i) => {
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
    const checkboxes: any[] = Array.from(
      document.querySelectorAll(baseSelector)
    );
    return typeof isSelected === "undefined"
      ? checkboxes
      : checkboxes.filter((checkbox) => checkbox.checked === isSelected);
  };

  const getSelectIds = (isSelected?: boolean) =>
    getSelectCheckboxes(isSelected).map((i) => i.id);

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

  const handleCreate = async (inputs: Record<string, any>) => {
    await handleRequest(admin?.endpoint ?? "", "POST", inputs, false);
    setReloadFlag(true);
  };

  const handleDelete = async (inputs: { ids: string[] }) => {
    await handleRequest(admin?.endpoint ?? "", "DELETE", inputs);
    setReloadFlag(true);
  };

  const handleDeleteModalShow = () => setShowDeleteModal(true);

  const handleDeleteModalCancel = () => setShowDeleteModal(false);

  const handleDeleteModalAccept = async () => {
    const ids = getSelectIds(true);
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
      keys.forEach((key) => {
        values[key.id] = key.value;
      });
    }
    await handleCreate(values);
  };

  const ColumnValue = ({
    columnSettings,
    record,
  }: {
    columnSettings: Column;
    record: any;
  }) => {
    const columnId: any = columnSettings.id ?? columnSettings.key ?? "";
    let columnValue = record[columnId];

    if (columnSettings.datetime && columnValue) {
      let dateFormat = columnSettings.datetime.dateFormat ?? "";
      let timeFormat = columnSettings.datetime.timeFormat ?? "";
      columnValue = moment(columnValue).format(`${dateFormat} ${timeFormat}`);
    } else if (columnSettings.mapper) {
      const mapper = columnSettings.mapper;
      if (typeof mapper === "function") {
        columnValue = mapper(columnValue);
      } else if (mapper.hasOwnProperty("label")) {
        let label = mapper["label"];

        if (typeof label !== "function") {
          columnValue = columnValue[label!];
        } else {
          columnValue = label(columnValue);
        }
      }
    }

    if (columnSettings.type === InputType.Boolean) {
      return (
        <Checkbox
          id={`${columnSettings.id}-${record.id}`}
          checked={columnValue}
        />
      );
    }

    return <>{columnValue}</>;
  };

  const getEditRowDefaultValue = (columnEditable?: { defaultValue?: any }) => {
    if (
      columnEditable?.defaultValue === undefined ||
      columnEditable?.defaultValue === null
    )
      return "";
    if (typeof columnEditable.defaultValue === "function") {
      return columnEditable.defaultValue();
    }
    return columnEditable.defaultValue;
  };

  const handleSelectAllChange = (e: CheckedState) => {
    const isSelected = e === true;
    setAllSelected(isSelected);
    let updatedItems = data.items.map((item) => ({
      ...item,
      isSelected: isSelected,
    }));
    setData((prevData) => ({
      ...prevData,
      items: updatedItems,
    }));
    setAnySelected(updatedItems.some((item) => item.isSelected));
  };

  const handleRowCheckChange = (id: number) => {
    const updatedItems = data.items.map((item) =>
      item.id === id ? { ...item, isSelected: !item.isSelected } : item
    );

    setData((prevData) => ({
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
          onClick={onAdd}
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
          columns.map((column: any, index: number) => {
            const columnId = column.key ?? column.id;
            if (column.editable) {
              return (
                <TableCell
                  className={cn(column.class, "centered")}
                  key={`${name}-${columnId}-${index}`}
                >
                  <Input
                    value={getEditRowDefaultValue(column.editable)}
                    settings={column}
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
          <ActionButton type={ACTION_BUTTON_KIND.add} onClick={onAdd} />
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
              columnsLength + (adminEnabled ? 1 : 0) + (adminAddEnabled ? 1 : 0)
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
    data,
    admin,
    adminAddEnabled,
  }: {
    data: any;
    admin: any;
    adminAddEnabled: boolean;
  }) => {
    return (
      <TableBody>
        {admin && adminAddEnabled && <EditRow />}
        {data.items.map((record: any, index: number) => {
          const rowId = `${name}-data-row-${index}`;
          return (
            <TableRow key={rowId} id={rowId} className={`${name}-data-row`}>
              {adminEnabled && (
                <TableCell>
                  <Checkbox
                    id={record.id}
                    checked={record.isSelected}
                    onChange={() => handleRowCheckChange(record.id)}
                  />
                </TableCell>
              )}
              {columns.map((column: Column, index: number) => (
                <TableCell key={index} className={column.class}>
                  <ColumnValue columnSettings={column} record={record} />
                </TableCell>
              ))}
              {adminAddEnabled && (
                <TableCell style={{ width: "100px", textAlign: "center" }}>
                  <ActionButton
                    type={ACTION_BUTTON_KIND.edit}
                    variant={VARIANTS.secondary}
                    // action={() => {}}
                    // dataId={record.id}
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
                <TableHead className={"w-[100px]"} style={{ width: "40px" }}>
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
              {adminEnabled && <TableHead className={"w-[100px]"} />}
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
              data={data}
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
