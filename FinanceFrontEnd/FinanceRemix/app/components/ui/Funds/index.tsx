import React, { useState } from "react";
import { useLoaderData } from "@remix-run/react";
import moment from "moment";
import urls from "@/utils/urls";
import Picker from "@/components/ui/utils/Picker";
import PaginatedTable, {
  Column,
} from "@/components/ui/utils/PaginatedTable";
import { InputType } from "@/components/ui/utils/InputType";
import CommonUtils from "@/utils/common";

// Define types for the props and states
interface PickerData {
  id: string;
  name: string;
}

interface LoaderData {
  banks: PickerData[];
  currencies: PickerData[];
}

const dateFormat = "DD/MM/YYYY";

const buildFundsEndpoint = (bankId: string, currencyId: string) => {
  const params = CommonUtils.Params({
    BankId: bankId,
    CurrencyId: currencyId,
  });
  return `${urls.funds.paginated}?${params}`;
};

const Funds: React.FC = () => {
  const { banks, currencies } = useLoaderData<LoaderData>();
  const [selectedBankId, setSelectedBankId] = useState<string>(banks[0].id);
  const [selectedCurrencyId, setSelectedCurrencyId] = useState<string>(
    currencies[0].id
  );
  const [fundsEndpoint, setFundsEndpoint] = useState<string>(
    buildFundsEndpoint(selectedBankId, selectedCurrencyId)
  );
  const [reloadTable, setReloadTable] = useState<boolean>(true);

  const updateFundsEndpoint = (bankId: string, currencyId: string) => {
    setFundsEndpoint(buildFundsEndpoint(bankId, currencyId));
  };

  const onBankPickerChange = (picker: { value: string }) => {
    var newBankId =
      selectedBankId !== picker.value ? picker.value : selectedBankId;
    setSelectedBankId(newBankId);
  };

  const onBankPickerFetch = ({ data }: { data: PickerData[] }) => {
    let newBankId = data[0].id;
    setSelectedBankId(newBankId);
  };

  const onCurrencyPickerChange = (picker: { value: string }) => {
    var newCurrencyId =
      selectedCurrencyId !== picker.value ? picker.value : selectedCurrencyId;
    setSelectedCurrencyId(newCurrencyId);
  };

  const onCurrencyPickerFetch = ({ data }: { data: PickerData[] }) => {
    let newCurrencyId = data[0].id;
    setSelectedCurrencyId(newCurrencyId);
  };

  const valueConditionalClass = {
    class: "text-success fw-bold",
    eval: (field: { value: number }) => field != null && field.value > 0,
  };

  const valueMapper = (field: { value: number }) =>
    field != null ? field.value : null;

  const numericHeader = {
    classes: "text-end",
    style: {
      width: "140px",
    },
  };

  const TableColumns: Column[] = [
    {
      id: "timeStamp",
      label: "Fecha",
      placeholder: "Fecha",
      type: InputType.DateTime,
      editable: {
        defaultValue: () => {
          let rowSelector = document.querySelector(
            ".bank-table-data-row > td > span"
          );

          if (!rowSelector?.textContent) return moment().format(dateFormat);

          return moment(rowSelector.textContent, `${dateFormat}`).format(
            dateFormat
          );
        },
      },
      datetime: {
        timeFormat: "HH:mm",
        timeIntervals: 15,
        dateFormat: dateFormat,
        placeholder: "Seleccionar fecha",
      },
      header: {
        style: {
          width: "160px",
        },
      },
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
        label: "name",
      },
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
        label: "name",
      },
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
        defaultValue: 0.0,
      },
      mapper: valueMapper,
      conditionalClass: valueConditionalClass,
    },
    {
      id: "dailyUse",
      label: "Use diario",
      placeholder: "Use diario",
      type: InputType.Boolean,
      header: {
        classes: "",
        style: {
          width: "80px",
        },
      },
      class: "text-center",
      editable: {
        defaultValue: false,
      },
    },
  ];

  // Update the endpoint whenever selectedBankId or selectedCurrencyId changes
  React.useEffect(() => {
    if (selectedBankId && selectedCurrencyId) {
      updateFundsEndpoint(selectedBankId, selectedCurrencyId);
      setReloadTable(true);
    }
  }, [selectedBankId, selectedCurrencyId, updateFundsEndpoint]);

  return (
    <>
      <div className="container pt-3 pb-3">
        <div className="row">
          <div className="col-6">
            <Picker
              id={"bank-picker"}
              data={banks}
              mapper={{ id: "id", label: (record) => `${record.name}` }}
              onChange={onBankPickerChange}
              onFetch={onBankPickerFetch}
            />
          </div>
          <div className="col-6">
            <Picker
              id={"currency-picker"}
              data={currencies}
              mapper={{ id: "id", label: (record) => `${record.name}` }}
              onChange={onCurrencyPickerChange}
              onFetch={onCurrencyPickerFetch}
            />
          </div>
        </div>
        <hr className="py-1" />
        {(!fundsEndpoint || fundsEndpoint.trim() === "") && (
          <div className={"container centered"}>Cargando datos</div>
        )}
        {fundsEndpoint && (
          <PaginatedTable
            name={"funds-table"}
            url={fundsEndpoint}
            reloadData={reloadTable}
            columns={TableColumns}
            admin={{
              endpoint: urls.funds.endpoint,
              key: [
                {
                  id: "BankId",
                  value: selectedBankId,
                },
                {
                  id: "CurrencyId",
                  value: selectedCurrencyId,
                },
              ],
            }}
          />
        )}
      </div>
    </>
  );
};

export default Funds;
