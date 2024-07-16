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
    id: "symbol",
    type: "TextInput",
    label: "Símbolo",
    placeholder: "Ingrese un símbolo",
    visible: true
  },
  {
    id: "description",
    type: "TextInput",
    label: "Descripción",
    placeholder: "Ingrese una descripción",
    visible: true
  },
  {
    id: "typeId",
    label: "Tipo",
    placeholder: "Seleccione un tipo de activo",
    type: "DropdownInput",
    endpoint: urls.iolInvestmentAssetTypes.endpoint,
    visible: true,
    mapper: {
      id: "id",
      label: "name"
    }
  },
  {
    id: "deactivated",
    type: "BooleanInput",
    label: "Desactivado",
    visible: true
  }
];

const TableSettings = {
  columns: FormInputSettings.filter(o => o.visible).map(o => {
    var result = { title: o.label, name: o.id };
    if (o.id === "typeId") {
      result["mapper"] = (r) => r.type.name;
    }
    return result;
  })
};

const IOLInvestmentsAsset = () => {
  return (
    <AdminModule
      moduleName={"IOLInvestmentsAsset"}
      title={"Activos de inversión"}
      formInputs={FormInputSettings}
      tableSettings={TableSettings}
      endpoint={urls.iolInvestmentAssets.endpoint}
    />
  );
};

export default IOLInvestmentsAsset;
