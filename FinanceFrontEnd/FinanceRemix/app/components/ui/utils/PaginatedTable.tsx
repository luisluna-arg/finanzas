import React, { useState, useEffect, useCallback } from "react";
import { Button, Form, FormCheck } from "react-bootstrap";
import moment from "moment";

import dates from "@/app/utils/dates";
import ActionButton, {
  ButtonType,
} from "@/app/components/ui/utils/ActionButton";
import ConfirmationModal from "@/app/components/ui/utils/ConfirmationModal";
import Input from "@/app/components/ui/utils/Input";
import LoadingSpinner from "@/app/components/ui/utils/LoadingSpinner";
import PaginationBar from "@/app/components/ui/utils/PaginationBar";
import { InputType } from "@/app/components/ui/utils/InputType";
import { fetchData } from "@/app/components/data/fetchData";
import { fetchPaginatedData } from "@/app/components/data/fetchPaginatedData";
import { handleRequest } from "@/app/components/data/handleRequest";
import { OUTLINE_VARIANT } from "@/app/components/ui/utils/Bootstrap/ColorVariant";

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

    const baseSelector = `table#${name} tr.${name}-data-row td:first-child input[type=checkbox]`;
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

  const getColumnValue = (columnSettings: Column, record: any) => {
    const columnId: any = columnSettings.id ?? columnSettings.key ?? "";
    const columnValue = record[columnId];

    if (columnSettings.datetime && columnValue) {
      let dateFormat = columnSettings.datetime.dateFormat ?? "";
      let timeFormat = columnSettings.datetime.timeFormat ?? "";
      return moment(columnValue).format(`${dateFormat} ${timeFormat}`);
    } else if (columnSettings.mapper) {
      const mapper = columnSettings.mapper;
      if (typeof mapper === "function") {
        return mapper(columnValue);
      }

      if (mapper.hasOwnProperty("label")) {
        let label = mapper["label"];

        if (typeof label !== "function") {
          return columnValue[label!];
        }

        return label(columnValue);
      }
    }
    return columnValue;
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

  const handleSelectAllChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const isSelected = e.target.checked;
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
        <ActionButton type={ButtonType.Add} classes={["me-2"]} action={onAdd} />
      );

    const DeleteButton = () => {
      return (
        adminDeletedEnabled && (
          <ActionButton
            type={ButtonType.Delete}
            variant={OUTLINE_VARIANT.DANGER}
            action={handleDeleteModalShow}
            disabled={!anySelected}
          />
        )
      );
    };

    return (
      <tr>
        {header ? (
          <th colSpan={columnCount}>
            <AddButton />
            <DeleteButton />
          </th>
        ) : (
          <td colSpan={columnCount}>
            <AddButton />
            <DeleteButton />
          </td>
        )}
      </tr>
    );
  };

  const EditRow = () => {
    return (
      <tr id={adminRowId} className={`${name}-edit-row`}>
        <td></td>
        {columns &&
          columns.map((column: any, index: number) => {
            const columnId = column.key ?? column.id;
            if (column.editable) {
              return (
                <td
                  className={`${column.class ?? ""} centered`}
                  key={`${name}-${columnId}-${index}`}
                >
                  <Input
                    value={getEditRowDefaultValue(column.editable)}
                    settings={column}
                  />
                </td>
              );
            } else {
              return (
                <td
                  className={"centered"}
                  key={`${name}-${columnId}-${index}`}
                ></td>
              );
            }
          })}
        <th className={"centered"}>
          <ActionButton type={ButtonType.Add} action={onAdd} />
        </th>
      </tr>
    );
  };

  const adminEnabled = adminAddEnabled || adminDeletedEnabled;
  const columnCount = columns.length + (adminEnabled ? 2 : 0);

  return (
    <div className="paginated-table ">
      <Form>
        <table id={name} className="table table-striped">
          <thead>
            <ActionsRow header={true} />
            <tr>
              {adminEnabled && (
                <th style={{ width: "40px" }}>
                  <FormCheck
                    id={`${name}-select-all`}
                    checked={allSelected}
                    onChange={handleSelectAllChange}
                  />
                </th>
              )}
              {columns.map((column, index) => {
                let classes = column?.header?.classes;
                classes = Array.isArray(classes) ? classes.join(" ") : classes;
                return (
                  <th
                    scope="col"
                    key={index}
                    className={classes}
                    style={column.header?.style}
                  >
                    {column.label}
                  </th>
                );
              })}
              {adminEnabled && <th />}
            </tr>
          </thead>
          {loading ? (
            <tbody>
              <tr>
                <td
                  colSpan={
                    columns.length +
                    (adminEnabled ? 1 : 0) +
                    (adminAddEnabled ? 1 : 0)
                  }
                >
                  <center>
                    <LoadingSpinner />
                  </center>
                </td>
              </tr>
            </tbody>
          ) : (
            <tbody>
              {admin && adminAddEnabled && <EditRow />}
              {data.items.map((record, index) => {
                const rowId = `${name}-data-row-${index}`;
                return (
                  <tr key={rowId} id={rowId} className={`${name}-data-row`}>
                    {adminEnabled && (
                      <td>
                        <FormCheck
                          id={record.id}
                          checked={record.isSelected}
                          onChange={() => handleRowCheckChange(record.id)}
                        />
                      </td>
                    )}
                    {columns.map((column, index) => (
                      <td key={index} className={column.class}>
                        {getColumnValue(column, record)}
                      </td>
                    ))}
                    {adminAddEnabled && (
                      <td style={{ width: "100px", textAlign: "center" }}>
                        <ActionButton
                          type={ButtonType.Edit}
                          variant={OUTLINE_VARIANT.WARNING}
                          action={() => {}}
                          dataId={record.id}
                        />
                      </td>
                    )}
                  </tr>
                );
              })}
            </tbody>
          )}
          <tfoot>
            <ActionsRow header={true} />
          </tfoot>
        </table>
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
