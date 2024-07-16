import React, { useEffect, useState } from "react";
import { Form, Table, Dropdown } from "react-bootstrap";
import Button from "../../../../utils/Button";
import ConfirmationModal from "../../../../utils/ConfirmationModal";
import CustomToast from "../../../../utils/CustomToast";
import FormModal from "../../../../utils/FormModal";

const TableModule = ({
  moduleName,
  title,
  formInputs,
  tableSettings,
  endpoint,
}) => {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [requestStatus, setRequestStatus] = useState(null);
  const [selectedItem, setSelectedItem] = useState(null);

  tableSettings.idColumn = tableSettings.idColumn ?? "id";

  const elementIds = {
    form: `${moduleName}-form`,
    table: `${moduleName}-table`,
  };

  const handleRequest = async (method, setRequestStatus, record) => {
    try {
      var headers = {
        "Content-Type": "application/json",
      };

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
      {requestStatus && <CustomToast variant="danger" text={requestStatus} />}
      <div className="flex-row">
        <Dropdown>
          <Dropdown.Toggle variant="success" id="dropdown-basic">
            Módulo
          </Dropdown.Toggle>

          <Dropdown.Menu>
            <Dropdown.Item href="#/action-1">BBVA</Dropdown.Item>
          </Dropdown.Menu>
        </Dropdown>
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
      <Table id={elementIds.table} className="table mt-2">
        <thead>
          <tr>
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

export default TableModule;
