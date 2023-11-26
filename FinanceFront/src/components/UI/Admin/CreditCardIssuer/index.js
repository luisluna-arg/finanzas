import React, { useEffect, useState } from "react";
import { Table } from "react-bootstrap";
import urls from "../../../../routing/urls";
import Button from "../../../utils/Button";
import CustomToast from "../../../utils/CustomToast";
import FormModal from "../../../utils/FormModal";

const fetchIssuers = async () => {
  let url = `${urls.creditCardIssuers.get}`;
  const response = await fetch(url);
  return await response.json();
};

const CreateIssuerSettings = [
  {
    id: "name",
    type: "TextInput",
    label: "Nombre",
    placeholder: "Ingrese un nombre",
  },
  //   { id: "bank", type: "DropdownInput", label: "Banco", placeholder: "Seleccione un banco" },
  { id: "deactivated", type: "BooleanInput", label: "Desactivado" },
];

const CreditCardIssuer = () => {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);

  const handleFormModalShow = () => setShowModal(true);
  const handleFormModalCancel = () => setShowModal(false);
  const handleFormModalAccept = () => {
    setShowModal(false);
  };

  useEffect(() => {
    setData([]);
    const fetchData = async () => {
      try {
        setData(await fetchIssuers());
      } catch (error) {
        console.error("Error fetching data:", error);
      } finally {
        setLoading(false);
      }
    };

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
      <h1>Emisores de tarjetas de crédito</h1>
      <div className="flex-row">
        <Button
          text={"Agregar"}
          className={"me-2"}
          onClickAction={handleFormModalShow}
        />
        <Button text={"Eliminar"} variant={"danger"} disabled={true} />
      </div>
      <FormModal
        show={showModal}
        title="Agregar Emisor de Tarjeta de Crédito"
        handleAccept={handleFormModalAccept}
        handleCancel={handleFormModalCancel}
        editorSettings={CreateIssuerSettings}
      />
      <Table className="table">
        <thead>
          <tr>
            <th scope="col">Banco</th>
            <th>Nombre</th>
            <th>Desactivado</th>
          </tr>
        </thead>
        <tbody>
          {data.map((issuer) => (
            <tr key={issuer.id}>
              <td>{issuer.Bank}</td>
              <td>{issuer.Name}</td>
              <td>{issuer.Deactivated}</td>
            </tr>
          ))}
        </tbody>
      </Table>
    </div>
  );
};

export default CreditCardIssuer;
