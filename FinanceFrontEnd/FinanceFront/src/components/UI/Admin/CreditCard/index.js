import React from "react";
import urls from "../../../../routing/urls";
import AdminModule from "../Utils/AdminModule";

const FormInputSettings = [
  {
    id: "id", 
    type: "TextInput", 
    label: "Id", 
    placeholder: null, 
    visible: false,
  },
  {
    id: "name",
    type: "TextInput",
    label: "Nombre",
    placeholder: "Ingrese un nombre",
  },
  {
    id: "bankId",
    label: "Banco",
    placeholder: "Seleccione un banco",
    type: "DropdownInput",
    endpoint: urls.banks.endpoint,
    mapper: {
      id: "id",
      label: "name"
    }
  },
  { id: "deactivated", type: "BooleanInput", label: "Desactivado" },
];

const TableSettings = {
  columns: [
    { title: "Nombre", name: "name" },
    { title: "Banco", name: "bankId", mapper: (r) => r.bank.name },
    { title: "Desactivado", name: "deactivated" },
  ],
};

const CreditCard = () => {
  return (
    <AdminModule
      moduleName={"CreditCard"}
      title={"Tarjetas de Crédito"}
      formInputs={FormInputSettings}
      tableSettings={TableSettings}
      endpoint={urls.creditCards.get}
    />
  );
};

export default CreditCard;
