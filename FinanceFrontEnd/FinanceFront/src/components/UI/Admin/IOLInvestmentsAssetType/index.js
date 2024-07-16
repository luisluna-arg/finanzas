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
    placeholder: "Ingrese un nombre",
    visible: true
  },
  {
    id: "deactivated",
    type: "BooleanInput",
    label: "Desactivado",
    visible: true
  }
];

const TableSettings = {
  columns: FormInputSettings.filter(o => o.visible).map(o => ({ title: o.label, name: o.id }))
};

const IOLInvestmentsAssetType = () => {
  return (
    <AdminModule
      moduleName={"IOLInvestmentsAssetType"}
      title={"Tipos de activos de inversión"}
      formInputs={FormInputSettings}
      tableSettings={TableSettings}
      endpoint={urls.iolInvestmentAssetTypes.endpoint}
    />
  );
};

export default IOLInvestmentsAssetType;
