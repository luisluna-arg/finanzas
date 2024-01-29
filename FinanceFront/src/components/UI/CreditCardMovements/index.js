import React, { useEffect, useState } from "react";
import urls from "../../../routing/urls";
import Uploader from "../../utils/Uploader";
import Picker from "../../utils/Picker";
import PaginatedTable from "../../utils/PaginatedTable";
import { InputControlTypes } from "../../utils/InputControl";

function CreditCardMovements() {
  const [selectedCreditCardId, setSelectedCreditCardId] = useState("");
  const [uploadEndpoint, setUploadEndpoint] = useState(`${urls.creditCardMovements.upload}`);
  const [movementsEndpoint, setMovementsEndpoint] = useState(``);

  const tableName = "credit-card-table";

  const updateMovementsEndpoint = (creditCardId) => {
    setMovementsEndpoint(`${urls.creditCardMovements.paginated}?CreditCardId=${creditCardId}`);
    setUploadEndpoint(`${urls.creditCardMovements.upload}?CreditCardId=${creditCardId}`);
  };

  const onCreditCardPickerChange = (picker) => {
    var newCreditCardId = selectedCreditCardId !== picker.value ? picker.value : selectedCreditCardId;
    setSelectedCreditCardId(newCreditCardId);
    updateMovementsEndpoint(newCreditCardId);
  };

  const onCreditCardPickerFetch = ({ data }) => {
    console.log(`onCreditCardPickerFetch`);
    let newCreditCardId = data[0].id;
    setSelectedCreditCardId(newCreditCardId);
    updateMovementsEndpoint(newCreditCardId);
  };

  const valueConditionalClass = {
    class: "text-success fw-bold",
    eval: (field) => field != null && field.value > 0
  };

  const valueMapper = (field) => field != null ? field.value : null;

  const numericHeader = {
    classes: "text-end",
    style: {
      width: "140px"
    }
  };

  const TableColumns = [
    {
      id: "timeStamp",
      label: "Fecha",
      placeholder: "Fecha",
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
      id: "concept",
      label: "Concepto",
      placeholder: "Concepto",
      editable: true
    },
    {
      id: "amount",
      label: "Pesos",
      placeholder: "Pesos",
      type: InputControlTypes.Decimal,
      min: 0.0,
      header: numericHeader,
      class: "text-end",
      editable: true,
      mapper: valueMapper,
      conditionalClass: valueConditionalClass
    },
    {
      id: "amountDollars",
      label: "Dólares",
      placeholder: "Dólares",
      type: InputControlTypes.Decimal,
      min: 0.0,
      header: numericHeader,
      class: "text-end",
      editable: true,
      mapper: valueMapper,
      conditionalClass: valueConditionalClass
    },
    {
      id: "paymentNumber",
      label: "Nro. Cuota",
      placeholder: "Nro. Cuota",
      type: InputControlTypes.Integer,
      min: 0,
      header: numericHeader,
      class: "text-end",
      editable: true,
    },
    {
      id: "planSize",
      label: "Cuotas",
      placeholder: "Cuotas",
      type: InputControlTypes.Integer,
      min: 0,
      header: numericHeader,
      class: "text-end",
      editable: true,
    }
  ];

  useEffect(() => {
  }, [selectedCreditCardId, movementsEndpoint]);


  return (
    <div className="container pt-3 pb-3">
      <div className="row">
        <div className="col">
          <Picker
            id={"credit-card-picker"}
            url={urls.creditCards.get}
            mapper={{ id: "id", label: record => `${record.name} ${record.bank.name}` }}
            onChange={onCreditCardPickerChange}
            onFetch={onCreditCardPickerFetch}
          />
        </div>
      </div>
      <hr className="py-1" />
      <Uploader url={uploadEndpoint} extensions={[".xls", ".xlsx"]} />
      <hr className="py-1" />
      {(!movementsEndpoint || movementsEndpoint.trim() === "") &&
        <div className={"container centered"}>Cargando datos</div>}
      {movementsEndpoint && <PaginatedTable
        name={tableName}
        admin={{
          endpoint: urls.creditCardMovements.endpoint,
          key: {
            id: "CreditCardId",
            value: selectedCreditCardId
          }
        }}
        url={movementsEndpoint}
        columns={TableColumns}
      />}
    </div>
  );
}

export default CreditCardMovements;
