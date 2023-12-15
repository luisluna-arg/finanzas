import React from "react";
import urls from "../../../../routing/urls";
import AdminModule from "../Utils/AdminModule";

const FormInputSettings = [
  {
    id: "id",
    type: "TextInput",
    label: "Id",
    placeholder: null,
    visible: false
  },
  {
    id: "name",
    type: "TextInput",
    label: "Nombre",
    placeholder: "Ingrese un nombre"
  },
  {
    id: "deactivated",
    type: "BooleanInput",
    label: "Desactivado"
  }
];

const TableSettings = {
  columns: [
    { title: "Nombre", name: "name" },
    { title: "Desactivado", name: "deactivated" },
  ],
};

const Bank = () => {
  return (
    <AdminModule
      moduleName={"Bank"}
      title={"Bancos"}
      formInputs={FormInputSettings}
      tableSettings={TableSettings}
      endpoint={urls.banks.endpoint}
    />
  );
};

export default Bank;
