import React, { useEffect, useState } from "react";
import moment from "moment";
import urls from "../../../routing/urls";
import Picker from "../../utils/Picker";
import PaginatedTable from "../../utils/PaginatedTable";
import { InputControlTypes } from "../../utils/InputControl";
import CommonUtils from "../../../utils/common";

function Incomes() {
  const [selectedBankId, setSelectedBankId] = useState("");
  const [selectedCurrencyId, setSelectedCurrencyId] = useState("");
  const [incomesEndpoint, setIncomesEndpoint] = useState(``);

  const tableName = "incomes-table";

  const dateFormat = "DD/MM/YYYY";

  const updateIncomesEndpoint = (bankId, currencyId) => {
    const params = CommonUtils.Params({
      BankId: bankId,
      CurrencyId: currencyId
    });
    setIncomesEndpoint(`${urls.incomes.paginated}?${params}`);
  };

  const onBankPickerChange = (picker) => {
    var newBankId = selectedBankId !== picker.value ? picker.value : selectedBankId;
    setSelectedBankId(newBankId);
  };

  const onBankPickerFetch = ({ data }) => {
    let newBankId = data[0].id;
    setSelectedBankId(newBankId);
  };

  const onCurrencyPickerChange = (picker) => {
    var newCurrencyId = selectedCurrencyId !== picker.value ? picker.value : selectedCurrencyId;
    setSelectedCurrencyId(newCurrencyId);
  };

  const onCurrencyPickerFetch = ({ data }) => {
    let newCurrencyId = data[0].id;
    setSelectedCurrencyId(newCurrencyId);
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

  const TableColumns = [{
    id: "timeStamp",
    label: "Fecha",
    placeholder: "Fecha",
    type: InputControlTypes.DateTime,
    editable: {
      defaultValue: () => {
        let rowSelector = document.querySelector(".bank-table-data-row > td > span");

        if (!rowSelector?.textContent) return moment().format(dateFormat);

        return moment(rowSelector.textContent, `${dateFormat}`).format(dateFormat);
      }
    },
    datetime: {
      timeFormat: "HH:mm",
      timeIntervals: 15,
      dateFormat: dateFormat,
      placeholder: "Seleccionar fecha",
    },
    header: {
      style: {
        width: "160px"
      }
    }
  },
  {
    id: "bank",
    label: "Banco/Entidad",
    placeholder: "Seleccione un banco",
    editable: false,
    type: InputControlTypes.Dropdown,
    endpoint: urls.banks.endpoint,
    mapper: {
      id: "id",
      label: "name"
    }
  },
  {
    id: "currency",
    label: "Moneda",
    placeholder: "Seleccione una moneda",
    editable: false,
    type: InputControlTypes.Dropdown,
    endpoint: urls.currencies.endpoint,
    mapper: {
      id: "id",
      label: "name"
    }
  },
  {
    id: "amount",
    label: "Pesos",
    placeholder: "Pesos",
    type: InputControlTypes.Decimal,
    min: 0.0,
    header: numericHeader,
    class: "text-end",
    editable: {
      defaultValue: 0.0
    },
    mapper: valueMapper,
    conditionalClass: valueConditionalClass
  }
  ];

  useEffect(() => {
    if (selectedBankId && selectedCurrencyId) {
      updateIncomesEndpoint(selectedBankId, selectedCurrencyId);
    }
  }, [selectedBankId, selectedCurrencyId]);

  return (
    <>
      <div className="container pt-3 pb-3">
        <div className="row">
          <div className="col-6">
            <Picker
              id={"bank-picker"}
              url={urls.banks.endpoint}
              mapper={{ id: "id", label: record => `${record.name}` }}
              onChange={onBankPickerChange}
              onFetch={onBankPickerFetch}
            />
          </div>
          <div className="col-6">
            <Picker
              id={"currency-picker"}
              url={urls.currencies.endpoint}
              mapper={{ id: "id", label: record => `${record.name}` }}
              onChange={onCurrencyPickerChange}
              onFetch={onCurrencyPickerFetch}
            />
          </div>
        </div>
        <hr className="py-1" />
        {(!incomesEndpoint || incomesEndpoint.trim() === "") &&
          <div className={"container centered"}>Cargando datos</div>}
        {incomesEndpoint && <PaginatedTable
          name={tableName}
          url={incomesEndpoint}
          columns={TableColumns}
          admin={{
            endpoint: urls.incomes.endpoint,
            key: [
              {
                id: "BankId",
                value: selectedBankId
              },
              {
                id: "CurrencyId",
                value: selectedCurrencyId
              }
            ]
          }}
        />}
      </div>
    </>
  );
}

export default Incomes;
