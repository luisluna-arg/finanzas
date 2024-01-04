// Movements.js
import React, { useEffect, useState } from "react";
import urls from "../../../routing/urls";
// import Uploader from "../../utils/Uploader";
// import Picker from "../../utils/Picker";
import PaginatedTable from "../../utils/PaginatedTable";
import { InputControlTypes } from "../../utils/InputControl";

function Movements() {
  const AppModuleTypeEnumFundsId = 1;

  const [selectedBankId, setSelectedBankId] = useState(null);
  const [selectedAppModuleId, setSelectedAppModuleId] = useState(null);
  const [fundsUploadEndpoint, setFundsUploadEndpoint] = useState(null);
  const [iolInvestmentsEndpoint, setIOLInvestmentsEndpoint] = useState(null);
  const [appModulesEndpoint, setAppModulesEndpoint] = useState(`${urls.appModules.endpoint}?AppModuleType=${AppModuleTypeEnumFundsId}`);

  const refreshEndpoints = () => {
    if (selectedAppModuleId && selectedBankId) {
      setFundsUploadEndpoint(`${urls.iolInvestments.upload}?DateKind=Local&AppModuleId=${selectedAppModuleId}&BankId=${selectedBankId}`);
    }
    setIOLInvestmentsEndpoint(`${urls.iolInvestments.paginated}`);
  }

  //   const onBankPickerChange = (picker) => {
  //     setSelectedBankId(picker.value);
  //     refreshEndpoints();
  //   };

  //   const onBankPickerFetch = (data) => {
  //     setSelectedBankId(data[0].id);
  //     refreshEndpoints();
  //   };

  //   const onAppModulePickerChange = (picker) => {
  //     setSelectedAppModuleId(picker.value);
  //     refreshEndpoints();
  //   };

  //   const onAppModulePickerFetch = (data) => {
  //     setSelectedAppModuleId(data[0].id);
  //     refreshEndpoints();
  //   };

  const onFetchInvestmentsTable = (data) => {

  }

  useEffect(() => {
    refreshEndpoints();
  }, [
    selectedBankId,
    selectedAppModuleId,
  ]);

  const numericColumn = (columnId, label) => ({
    id: columnId,
    label: label,
    placeholder,
    headerClass: "text-end",
    class: "text-end",
    editable: true,
    mapper: (field) => parseFloat(field.toFixed(2)),
    conditionalClass: {
      class: "text-success fw-bold",
      eval: (field) => field > 0
    }
  });

  const placeholder = "Ingrese un valor";
  const investmentsTableColumns = [
    {
      id: "timeStamp",
      label: "Fecha",
      placeholder,
      type: InputControlTypes.DateTime,
      editable: true,
      datetime: {
        timeFormat: "HH:mm",
        timeIntervals: 15,
        dateFormat: "DD/MM/YYYY HH:mm",
        placeholder: "Seleccionar fecha",
      },
      header: {
        style: {
          width: "160px"
        }
      }
    },
    {
      id: "asset",
      label: "Activo",
      placeholder,
      editable: true,
      mapper: (field) => `${field.symbol} - ${field.description}`,
    },
    numericColumn("alarms", "Alarmas"),
    numericColumn("quantity", "Cantidad"),
    numericColumn("assets", "Activos comp."),
    numericColumn("dailyVariation", "Variación diaria"),
    numericColumn("lastPrice", "Último precio"),
    numericColumn("averageBuyPrice", "Precio prom. compra"),
    numericColumn("averageReturnPercent", "Rendimiento (%)"),
    numericColumn("averageReturn", "Rendimiento prom."),
    numericColumn("valued", "Valorado")
  ];

  let enabled = iolInvestmentsEndpoint
    // && fundsUploadEndpoint
    // && appModulesEndpoint
    ;

  return (
    <>
      <div className="container pt-3 pb-3">
        {/* <div className="row">
          <div className="col">
            <Picker
              id={"bank-picker"}
              url={urls.banks.endpoint}
              mapper={{ id: "id", label: "name" }}
              onChange={onBankPickerChange}
              onFetch={onBankPickerFetch} />
          </div>
          <div className="col-3">
            <Picker
              id={"module-picker"}
              url={appModulesEndpoint}
              mapper={{ id: "id", label: "name" }}
              onChange={onAppModulePickerChange}
              onFetch={onAppModulePickerFetch}
            />
          </div>
        </div> */}
        {!enabled && <div>Cargando...</div>}
        {enabled && <div>
          {/* <hr className="py-1" />
          <Uploader url={fundsUploadEndpoint} extensions={[".xls", ".xlsx"]} /> */}
          <hr className="py-1" />
          <PaginatedTable
            name={"investments-table"}
            url={iolInvestmentsEndpoint}
            // admin={{
            //   endpoint: urls.iolInvestments.endpoint,
            //   key: [
            //     {
            //       id: "AppModuleId",
            //       value: selectedAppModuleId
            //     },
            //     {
            //       id: "BankId",
            //       value: selectedBankId
            //     }
            //   ]
            // }}
            onFetch={onFetchInvestmentsTable}
            columns={investmentsTableColumns} />
        </div>
        }
      </div>
    </>
  );
}

export default Movements;
