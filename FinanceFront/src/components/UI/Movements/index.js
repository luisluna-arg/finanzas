import React, { useEffect, useState } from "react";
import urls from "../../../routing/urls";
import Uploader from "../../utils/Uploader";
import Picker from "../../utils/Picker";
import PaginatedTable from "../../utils/PaginatedTable";
import { InputControlTypes } from "../../utils/InputControl";

function Movements() {
  const AppModuleTypeEnumFundsId = 1;

  const [selectedBankId, setSelectedBankId] = useState(null);
  const [selectedAppModuleId, setSelectedAppModuleId] = useState(null);
  const [fundsUploadEndpoint, setFundsUploadEndpoint] = useState(null);
  const [movementsEndpoint, setMovementsEndpoint] = useState(null);
  const [appModulesEndpoint, setAppModulesEndpoint] = useState(`${urls.appModules.endpoint}?AppModuleType=${AppModuleTypeEnumFundsId}`);

  const refreshEndpoints = () => {
    if (selectedAppModuleId && selectedBankId) {
      setFundsUploadEndpoint(`${urls.movements.upload}?DateKind=Local&AppModuleId=${selectedAppModuleId}&BankId=${selectedBankId}`);
      setMovementsEndpoint(`${urls.movements.paginated}?AppModuleId=${selectedAppModuleId}&BankId=${selectedBankId}`);
    }
  }

  const onBankPickerChange = (picker) => {
    setSelectedBankId(picker.value);
    refreshEndpoints();
  };

  const onBankPickerFetch = (data) => {
    setSelectedBankId(data.data[0].id);
    refreshEndpoints();
  };

  const onAppModulePickerChange = (picker) => {
    setSelectedAppModuleId(picker.value);
    refreshEndpoints();
  };

  const onAppModulePickerFetch = (data) => {
    setSelectedAppModuleId(data.data[0].id);
    refreshEndpoints();
  };

  const onFetchFundsTable = (data) => {

  }

  useEffect(() => {
    refreshEndpoints();
  }, [
    selectedBankId,
    selectedAppModuleId,
    refreshEndpoints
  ]);

  const placeholder = "Ingrese un valor";
  const textStyle = {
    paddingTop: '0',
    paddingBottom: '0',
  };
  const numericStyle = {
    paddingTop: '0',
    paddingBottom: '0',
  };
  const movementsTableColumns = [
    {
      id: "timeStamp",
      label: "Fecha",
      placeholder,
      type: InputControlTypes.DateTime,
      editable: true,
      style: {
        paddingTop: '0',
        paddingBottom: '0',
      },
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
      id: "concept1",
      label: "Concepto 1",
      placeholder,
      editable: true,
      style: textStyle,
    },
    {
      id: "concept2",
      label: "Concepto 2",
      placeholder,
      editable: true,
      style: textStyle,
    },
    {
      id: "amount",
      label: "Monto",
      placeholder,
      headerClass: "text-end",
      class: "text-end",
      editable: true,
      style: numericStyle,
      mapper: (field) => parseFloat(field.value.toFixed(2)),
      conditionalClass: {
        class: "text-success fw-bold",
        eval: (field) => field.value > 0
      }
    },
    {
      id: "total",
      label: "Total",
      placeholder,
      headerClass: "text-end",
      class: "text-end",
      editable: true,
      style: numericStyle,
      mapper: (field) => parseFloat(field.value.toFixed(2)),
    }
  ];

  let enabled = fundsUploadEndpoint
    && movementsEndpoint
    && appModulesEndpoint;

  return (
    <>
      <div className="container pt-3 pb-3">
        <div className="row">
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
        </div>
        {!enabled && <div>Cargando...</div>}
        {enabled && <div>
          <hr className="py-1" />
          <Uploader url={fundsUploadEndpoint} extensions={[".xls", ".xlsx"]} />
          <hr className="py-1" />
          <PaginatedTable
            name={"funds-table"}
            url={movementsEndpoint}
            admin={{
              endpoint: urls.movements.endpoint,
              key: [
                {
                  id: "AppModuleId",
                  value: selectedAppModuleId
                },
                {
                  id: "BankId",
                  value: selectedBankId
                }
              ]
            }}
            onFetch={onFetchFundsTable}
            columns={movementsTableColumns} />
        </div>
        }
      </div>
    </>
  );
}

export default Movements;
