import React, { useState } from "react";
import { useLoaderData } from "@remix-run/react";
import moment from "moment";
import urls from "@/app/utils/urls";
import CommonUtils from "@/app/utils/common";
import { InputType } from "@/app/components/ui/utils/InputType";
import Picker from "@/app/components/ui/utils/Picker";
import PaginatedTable, { Column, ConditionalClass } from "@/app/components/ui/utils/PaginatedTable";

// Define types for the props and states
interface PickerData {
  id: string;
  name: string;
}

interface LoaderData {
  banks: PickerData[];
  currencies: PickerData[];
}

const dateFormat = "DD/MM/yyyy";

const buildIncomesEnpoint = (bankId: string, currencyId: string) => {
  const params = CommonUtils.Params({
    BankId: bankId,
    CurrencyId: currencyId
  });
  return `${urls.incomes.paginated}?${params}`;
}

const Incomes: React.FC = () => {
  const { banks, currencies } = useLoaderData<LoaderData>();
  const [selectedBankId, setSelectedBankId] = useState<string>(banks[0].id);
  const [selectedCurrencyId, setSelectedCurrencyId] = useState<string>(currencies[0].id);
  const [incomesEndpoint, setIncomesEndpoint] = useState<string>(buildIncomesEnpoint(selectedBankId, selectedCurrencyId));
  const [reloadTable, setReloadTable] = useState<boolean>(true);

  const updateIncomesEndpoint = /*useCallback(*/(bankId: string, currencyId: string) => {
    setIncomesEndpoint(buildIncomesEnpoint(bankId, currencyId));
  }/*, [])*/;

  const onBankPickerChange = (picker: { value: string }) => {
    setSelectedBankId(picker.value);
    setIncomesEndpoint(buildIncomesEnpoint(picker.value, selectedCurrencyId));
  };

  const onBankPickerFetch = ({ data }: { data: PickerData[] }) => {
    const newBankId = data[0]?.id;
    setSelectedBankId(newBankId);
  };

  const onCurrencyPickerChange = (picker: { value: string }) => {
    setSelectedCurrencyId(picker.value);
    setIncomesEndpoint(buildIncomesEnpoint(selectedBankId, picker.value));
  };

  const onCurrencyPickerFetch = ({ data }: { data: PickerData[] }) => {
    const newCurrencyId = data[0]?.id;
    setSelectedCurrencyId(newCurrencyId);
  };

  const valueConditionalClass: ConditionalClass = {
    class: "text-success fw-bold",
    eval: (field: { value: number }) => field != null && field.value > 0
  };

  const valueMapper = (field: { value: number }) => field != null ? field.value : null;

  const numericHeader = {
    classes: "text-end",
    style: {
      width: "140px"
    }
  };

  const TableColumns: Column[] = [
    {
      id: "createdAt",
      label: "Fecha",
      placeholder: "Fecha",
      type: InputType.DateTime,
      editable: {
        defaultValue: () => {
          const rowSelector = document.querySelector(".bank-table-data-row > td > span");

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
      type: InputType.Dropdown,
      endpoint: urls.banks.endpoint,
      mapper: {
        id: "id",
        label: (record: PickerData) => `${record.name}`
      }
    },
    {
      id: "currency",
      label: "Moneda",
      placeholder: "Seleccione una moneda",
      editable: false,
      type: InputType.Dropdown,
      endpoint: urls.currencies.endpoint,
      mapper: {
        id: "id",
        label: (record: PickerData) => `${record.name}`
      }
    },
    {
      id: "amount",
      label: "Pesos",
      placeholder: "Pesos",
      type: InputType.Decimal,
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

  // Update the endpoint whenever selectedBankId or selectedCurrencyId changes
  React.useEffect(() => {
    if (selectedBankId && selectedCurrencyId) {
      updateIncomesEndpoint(selectedBankId, selectedCurrencyId);
      setReloadTable(true);
    }
  }, [selectedBankId, selectedCurrencyId, updateIncomesEndpoint]);

  return (
    <>
      <div className="container pt-3 pb-3">
        <div className="row">
          <div className="col-6">
            <Picker
              id="bank-picker"
              value={selectedBankId}
              data={banks}
              mapper={{ id: "id", label: record => `${record.name}` }}
              onChange={onBankPickerChange}
              onFetch={onBankPickerFetch}
            />
          </div>
          <div className="col-6">
            <Picker
              id="currency-picker"
              value={selectedCurrencyId}
              data={currencies}
              mapper={{ id: "id", label: record => `${record.name}` }}
              onChange={onCurrencyPickerChange}
              onFetch={onCurrencyPickerFetch}
            />
          </div>
        </div>
        <hr className="py-1" />
        {(!incomesEndpoint || incomesEndpoint.trim() === "") &&
          <div className="container centered">Cargando datos</div>}
        {incomesEndpoint && <PaginatedTable
            name="incomes-table"
            url={incomesEndpoint}
            reloadData={reloadTable}
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
          />
        }
      </div>
    </>
  );
};

export default Incomes;
