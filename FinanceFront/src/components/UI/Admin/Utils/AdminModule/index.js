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
  const [selectedItems, setSelectedItems] = useState(false);
  const [editEnabled, setEditEnabled] = useState(false);
  const [deleteEnabled, setDeleteEnabled] = useState(false);
  const [showEditModal, setShowEditModal] = useState(false);
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [error, setError] = useState(null);
  const [createStatus, setCreateStatus] = useState(null);

  const elementIds = {
    form: `${moduleName}-form`,
    table: `${moduleName}-table`,
  };

  const handleRequest = async (method, setCreateStatus, inputs) => {
    try {
      let record = {};
      Array.from(inputs).map((i) => (record[i.id] = i.value));

      const response = await fetch(endpoint, {
        method: method,
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(record),
      });

      if (response.ok) {
        setCreateStatus(null);
      } else {
        setCreateStatus("Ocurrió un error en la operación");
      }
    } catch (error) {
      console.error("Error:", error);
      setCreateStatus("Ocurrió un error en la operación");
    }
  };

  const handleCreate = async (setCreateStatus, inputs) => {
    await handleRequest("POST", setCreateStatus, inputs);
  };

  const handleUpdate = async (setCreateStatus, inputs) => {
    await handleRequest("PUT", setCreateStatus, inputs);
  };

  const handleDelete = async (setCreateStatus, ids) => {
    await handleRequest("DELETE", setCreateStatus, { ids: ids });
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
    let inputs = table.querySelectorAll("input:checked");
    let itemCount = inputs.length;
    setSelectedItems(itemCount);
    setDeleteEnabled(itemCount > 0);
    setEditEnabled(itemCount === 1);
  };

  const clearForm = () => {
    let form = document.getElementById(elementIds.form);
    let inputs = form.getElementsByTagName("input");
    setError(null);
    for (let i = 0; i < inputs.length; i++) {
      inputs[i].value = "";
    }
  };

  const handleEditModalShow = () => setShowEditModal(true);
  const handleEditModalCancel = () => setShowEditModal(false);
  const handleEditModalAccept = async () => {
    let form = document.getElementById(elementIds.form);
    let inputs = form.getElementsByTagName("input");
    setError(null);
    for (let i = 0; i < inputs.length; i++) {
      if (inputs[i].type !== "checkbox" && !inputs[i].value) {
        setError("Algunos campos no han sido completados");
        break;
      }
    }

    let action = editEnabled ? handleUpdate : handleCreate;

    await action(setCreateStatus, inputs);

    if (!createStatus) {
      setShowEditModal(false);
      clearForm();
      fetchData();
    }
  };

  const handleDeleteModalShow = () => {
    setShowDeleteModal(true);
    console.log("ADADSASD");
    console.log(`${showDeleteModal}`);
  };

  const handleDeleteModalCancel = () => setShowDeleteModal(false);

  const handleDeleteModalAccept = async () => {
    const inputs = Array.from(
      document
        .getElementById(elementIds.table)
        .querySelectorAll(".id-checkbox input:checked")
    );

    let inputIds = inputs.map((i) => i.id);

    // await handleDelete(
    //   setCreateStatus,
    //   inputIds
    // );

    // if (!createStatus) {
    //   setShowDeleteModal(false);
    //   clearForm();
    //   fetchData();
    // }

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
      <h1>{title}</h1>
      {createStatus && <CustomToast variant="danger" text={createStatus} />}
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
                  var columnKey = `${rowKey}-col-${colIndex}`;

                  if (typeof record[column.name] === "boolean") {
                    return (
                      <td key={columnKey}>
                        <Form.Check
                          type="checkbox"
                          value={record[column.name]}
                          disabled
                        />
                      </td>
                    );
                  } else {
                    return <td key={columnKey}>{record[column.name]}</td>;
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
