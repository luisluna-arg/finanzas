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
    id: "shortName",
    type: "TextInput",
    label: "Nombre corto",
    placeholder: "Ingrese un nombre corto"
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
    { title: "Nombre Corto", name: "shortName" },
    { title: "Desactivado", name: "deactivated" },
  ],
};

const Currency = () => {
  return (
    <AdminModule
      moduleName={"Currency"}
      title={"Monedas"}
      formInputs={FormInputSettings}
      tableSettings={TableSettings}
      endpoint={urls.currencies.endpoint}
    />
  );
};

export default Currency;
