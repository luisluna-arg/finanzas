import React, { useEffect, useState } from "react";
import { Form, Table } from "react-bootstrap";
import Button from "../../../../utils/Button";
import ConfirmationModal from "../../../../utils/ConfirmationModal";
import CustomToast from "../../../../utils/CustomToast";
import FormModal from "../../../../utils/FormModal";

const AdminModule = ({
  moduleName,
  title,
  formInputs,
  tableSettings,
  endpoint,
}) => {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [selectedInputs, setSelectedInputs] = useState([]);
  const [selectedItemCount, setSelectedItemCount] = useState(0);
  const [editEnabled, setEditEnabled] = useState(false);
  const [deleteEnabled, setDeleteEnabled] = useState(false);
  const [showEditModal, setShowEditModal] = useState(false);
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [error, setError] = useState(null);
  const [requestStatus, setRequestStatus] = useState(null);
  const [selectedItem, setSelectedItem] = useState(null);

  tableSettings.idColumn = tableSettings.idColumn ?? "id";

  const elementIds = {
    form: `${moduleName}-form`,
    table: `${moduleName}-table`,
  };

  const clearForm = () => {
    let inputs = getFormInputs();
    setError(null);
    for (let i = 0; i < inputs.length; i++) {
      inputs[i].value = "";
    }
  };

  const validateForm = (inputs) => {
    setError(null);
    inputs = inputs ?? getFormInputs();
    for (let i = 0; i < inputs.length; i++) {
      var input = inputs[i];
      if (
        !input.classList.contains("invisible") &&
        input.type !== "checkbox" &&
        !input.value
      ) {
        setError("Algunos campos no han sido completados");
        break;
      }
    }
  };

  const getFormInputs = () => {
    let form = document.getElementById(elementIds.form);
    return [
      ...Array.from(form.getElementsByTagName("input")),
      ...Array.from(form.getElementsByTagName("select"))
    ];
  };

  const getFormValues = () => {
    return getFormInputs().reduce((o, i) => {
      if (i && i.type === "checkbox") {
        o[i.id] = i.checked;
      }
      else {
        o[i.id] = i.value;
      }

      return o;
    }, {});
  }

  const handleRequest = async (method, setRequestStatus, record) => {
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

      const response = await fetch(endpoint, {
        method: method,
        headers: headers,
        body: JSON.stringify(record),
      });

      if (response.ok) {
        setRequestStatus(null);
      } else {
        setRequestStatus("Ocurrió un error en la operación");
      }
    } catch (error) {
      console.error("Error:", error);
      setRequestStatus("Ocurrió un error en la operación");
    }
  };

  const handleCreate = async (setRequestStatus, inputs) => {
    await handleRequest("POST", setRequestStatus, inputs);
  };

  const handleUpdate = async (setRequestStatus, inputs) => {
    await handleRequest("PUT", setRequestStatus, inputs);
  };

  const handleDelete = async (setRequestStatus, ids) => {
    await handleRequest("DELETE", setRequestStatus, { ids: ids });
  };

  const fetchData = async () => {
    setData([]);
    try {
      let data = await fetch(endpoint);
      setData(await data.json());
    } catch (error) {
      console.error("Error fetching data:", error);
    } finally {
      setLoading(false);
    }
  };

  const onItemSelect = () => {
    let table = document.getElementById(elementIds.table);
    let inputs = Array.from(table.querySelectorAll("input:checked"));
    let itemCount = inputs.length;
    setSelectedInputs(inputs);
    setSelectedItemCount(itemCount);
    setDeleteEnabled(itemCount > 0);
    setEditEnabled(itemCount === 1);
  };

  const handleEditModalShow = () => {
    setError(null);

    if (selectedItemCount === 0) {
      setShowEditModal(true);
      setSelectedItem(null);
    } else {
      let selectedId = selectedInputs[0].id;
      let selectedItem = data.find(
        (o) => o[tableSettings.idColumn] === selectedId
      );

      if (selectedItem) {
        setShowEditModal(true);
        setSelectedItem(selectedItem);
      } else {
        setError("No se ha encontrado el registro a editar");
      }
    }
  };

  const handleEditModalCancel = () => {
    setShowEditModal(false);
    setSelectedItem(null);
  };

  const handleEditModalAccept = async () => {
    validateForm();

    let action = editEnabled ? handleUpdate : handleCreate;

    await action(setRequestStatus, getFormValues());

    if (!requestStatus) {
      setShowEditModal(false);
      setSelectedItem(null);
      clearForm();
      fetchData();
    }
  };

  const handleDeleteModalShow = () => {
    setShowDeleteModal(true);
  };

  const handleDeleteModalCancel = () => setShowDeleteModal(false);

  const handleDeleteModalAccept = async () => {
    const inputs = Array.from(
      document
        .getElementById(elementIds.table)
        .querySelectorAll(".id-checkbox input:checked")
    );

    let inputIds = inputs.map((i) => i.id);

    await handleDelete(setRequestStatus, inputIds);

    if (!requestStatus) {
      setShowDeleteModal(false);
      fetchData();
    }

    setShowDeleteModal(false);
  };

  useEffect(() => {
    // Call the fetchData function when the component mounts
    fetchData();
  }, []);

  if (loading) {
    return (
      <CustomToast text={"Cargando..."} position="top-center"></CustomToast>
    );
  }

  return (
    <div className="p-3 d-flex flex-column">
      {title && (<h1>{title}</h1>)}
      {requestStatus && <CustomToast variant="danger" text={requestStatus} />}
      <div className="flex-row">
        <Button
          text={"Agregar"}
          className={"me-2"}
          onClickAction={handleEditModalShow}
        />
        <Button
          text={"Editar"}
          variant={"warning"}
          className={"me-2"}
          disabled={!editEnabled}
          onClickAction={handleEditModalShow}
        />
        <Button
          text={"Eliminar"}
          variant={"danger"}
          disabled={!deleteEnabled}
          onClickAction={handleDeleteModalShow}
        />
      </div>
      <FormModal
        formId={elementIds.form}
        show={showEditModal}
        title="Agregar"
        error={error}
        handleAccept={handleEditModalAccept}
        handleCancel={handleEditModalCancel}
        editorSettings={formInputs}
        form={selectedItem ?? {}}
      />
      <ConfirmationModal
        text="¿Confirma la eliminacion del registro?"
        show={showDeleteModal}
        handleAccept={handleDeleteModalAccept}
        handleCancel={handleDeleteModalCancel}
      />
      <Table id={elementIds.table} className="table mt-2">
        <thead>
          <tr>
            <th></th>
            {tableSettings.columns.map((column, rowIndex) => {
              const rowKey = `${moduleName}-header-${rowIndex}`;
              return <th key={rowKey}>{column.title}</th>;
            })}
          </tr>
        </thead>
        <tbody>
          {data.map((record, rowIndex) => {
            const rowKey = `${moduleName}-row-${rowIndex}`;
            return (
              <tr key={rowKey}>
                <td>
                  <Form.Check
                    id={`${record[tableSettings.idColumn ?? "id"]}`}
                    type="checkbox"
                    className="id-checkbox"
                    onClick={onItemSelect}
                  />
                </td>
                {tableSettings.columns.map((column, colIndex) => {
                  let columnKey = `${rowKey}-col-${colIndex}`;
                  let columnValue = record[column.name];
                  columnValue = column.mapper ? column.mapper(record) : columnValue;

                  if (typeof columnValue === "boolean") {
                    return (
                      <td key={columnKey}>
                        <Form.Check
                          type="checkbox"
                          value={columnValue}
                          disabled
                        />
                      </td>
                    );
                  } else {
                    return <td key={columnKey}>{columnValue}</td>;
                  }
                })}
              </tr>
            );
          })}
        </tbody>
      </Table>
    </div>
  );
};

export default AdminModule;
