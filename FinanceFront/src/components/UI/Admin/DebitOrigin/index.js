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
    id: "appModuleId",
    label: "Módulo",
    placeholder: "Seleccione un módulo",
    type: "DropdownInput",
    endpoint: urls.appModules.endpoint,
    mapper: {
      id: "id",
      label: "name"
    }
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

const DebitOrigin = () => {
  return (
    <AdminModule
      moduleName={"DebitOrigin"}
      title={"Orígenes de débito"}
      formInputs={FormInputSettings}
      tableSettings={TableSettings}
      endpoint={urls.debitOrigins.endpoint}
    />
  );
};

export default DebitOrigin;
