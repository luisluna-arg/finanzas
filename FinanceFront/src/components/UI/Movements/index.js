// Movements.js
import React, { useEffect, useState } from "react";
import urls from "../../../routing/urls";
import Uploader from "../../utils/Uploader";
import Picker from "../../utils/Picker";
import PaginatedTable from "../../utils/PaginatedTable";

function Movements() {
  const [selectedBankId, setSelectedBankId] = useState("");
  const [selectedAppModuleId, setSelectedAppModuleId] = useState("");
  const [fundsUploadEndpoint, setFundsUploadEndpoint] = useState(`${urls.funds.upload}`);
  const [movementsEndpoint, setMovementsEndpoint] = useState(``);

  const refreshEndpoints = () => {
    setFundsUploadEndpoint(`${urls.movements.upload}?DateKind=Local&AppModuleId=${selectedAppModuleId}&BankId=${selectedBankId}`);
    setMovementsEndpoint(`${urls.movements.paginated}?AppModuleId=${selectedAppModuleId}&BankId=${selectedBankId}`);
  }

  const onBankPickerChange = (picker) => {
    setSelectedBankId(picker.value);
  };

  const onBankPickerFetch = (data) => {
    setSelectedBankId(data[0].id);
  };

  const onAppModulePickerChange = (picker) => {
    setSelectedAppModuleId(picker.value);
  };

  const onAppModulePickerFetch = (data) => {
    setSelectedAppModuleId(data[0].id);
  };

  const onFetchFundsTable = (data) => {
    
  }

  useEffect(() => {
    refreshEndpoints();
  }, [selectedBankId, selectedAppModuleId]);

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
              url={urls.appModules.endpoint}
              mapper={{ id: "id", label: "name" }}
              onChange={onAppModulePickerChange}
              onFetch={onAppModulePickerFetch}
            />
          </div>
        </div>
        <hr className="py-1" />
        <Uploader url={fundsUploadEndpoint} extensions={[".xls", ".xlsx"]} />
        <hr className="py-1" />
        <PaginatedTable
          name={"funds-table"}
          url={movementsEndpoint}
          onFetch={onFetchFundsTable}
          columns={[
            {
              id: "timeStamp",
              label: "Fecha",
              type: "datetime"
            },
            {
              id: "concept1",
              label: "Concepto 1",
            },
            {
              id: "concept2",
              label: "Concepto 2",
            },
            {
              id: "amount",
              label: "Monto",
              headerClass: "text-end",
              class: "text-end",
              mapper: (field) => parseFloat(field.value.toFixed(2)),
              conditionalClass: {
                class: "text-success fw-bold",
                eval: (field) => field.value > 0
              }
            },
            {
              id: "total",
              label: "Total",
              headerClass: "text-end",
              class: "text-end",
              mapper: (field) => parseFloat(field.value.toFixed(2)),
            }
          ]} />
      </div>
    </>
  );
}

export default Movements;
