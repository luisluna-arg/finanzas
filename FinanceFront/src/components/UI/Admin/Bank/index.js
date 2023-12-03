import React from "react";
import urls from "../../../../routing/urls";
import AdminModule from "../Utils/AdminModule";
import FormInput from "../Utils/Helpers/FormInput";

const FormInputSettings = [
  new FormInput("name", "TextInput", "Nombre", "Ingrese un nombre"),
  new FormInput("deactivated", "BooleanInput", "Desactivado"),
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
      endpoint={urls.Banks.endpoint}
    />
  );
};

export default Bank;
