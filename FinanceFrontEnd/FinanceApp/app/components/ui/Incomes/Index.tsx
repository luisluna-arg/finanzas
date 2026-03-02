import React from 'react';
import urls from '@/utils/urls';
import dayjs from 'dayjs';
import CommonUtils from '@/utils/common';
import BankCurrencySelector from '@/components/ui/utils/BankCurrencySelector';
import { useLoaderData, useLocation, useNavigate } from 'react-router';
import { InputType } from '@/components/ui/utils/InputType';
import PaginatedTable, {
  Column,
  ConditionalClass,
  Row,
} from '@/components/ui/utils/PaginatedTable';
import { cn } from '@/lib/utils';

// Define types for the props and states
interface PickerData {
  id: string;
  name: string;
}

interface BankData {
  id: string;
  name: string;
  deactivated: boolean;
  creditCards: unknown[];
}

interface CurrencyData {
  id: string;
  name: string;
  shortName: string;
  deactivated: boolean;
  baseExchangeRates: unknown[];
  quoteExchangeRates: unknown[];
  iolInvestmentAssets: unknown[];
  subscriptions: unknown[];
  symbols: unknown[];
}

interface IncomeRecord extends Row {
  id: string;
  createdAt: string;
  updatedAt: string;
  timeStamp: string;
  deactivated: boolean;
  amount: number;
  bankId: string;
  currencyId: string;
  bank: BankData;
  currency: CurrencyData;
}

interface LoaderData {
  banks: PickerData[];
  currencies: PickerData[];
  data: IncomeRecord[];
  bankId: string;
  currencyId: string;
}

const dateFormat = 'DD/MM/YYYY';

const Incomes: React.FC = () => {
  const { banks, currencies, data, bankId, currencyId } = useLoaderData<LoaderData>();
  const navigate = useNavigate();
  const location = useLocation();

  const reload = ({
    currentBankId,
    currentCurrencyId,
  }: {
    currentBankId?: string;
    currentCurrencyId?: string;
  }) => {
    const params = CommonUtils.Params({
      bankId: currentBankId ?? bankId,
      currencyId: currentCurrencyId ?? currencyId,
    });
    navigate(`${location.pathname}?${params}`);
  };

  const onBankPickerChange = (picker: { value: string }) => {
    reload({ currentBankId: picker.value });
  };

  const onCurrencyPickerChange = (picker: { value: string }) => {
    reload({ currentCurrencyId: picker.value });
  };

  const valueConditionalClass: ConditionalClass = {
    class: 'text-success fw-bold',
    eval: (field: unknown) => field != null && Number(String(field)) > 0,
  };

  const valueMapper = function (field: unknown) {
    return field != null ? Number(String(field)) : null;
  };

  const numericHeader = {
    classes: 'text-end',
    style: {
      width: '180px',
    },
  };

  const TableColumns: Column<IncomeRecord>[] = [
    {
      id: 'createdAt',
      label: 'Fecha',
      placeholder: 'Fecha',
      type: InputType.DateTime,
      editable: {
        defaultValue: () => dayjs().format(dateFormat),
      },
      datetime: {
        timeFormat: 'HH:mm',
        timeIntervals: 15,
        dateFormat: dateFormat,
        placeholder: 'Seleccionar fecha',
      },
      header: {
        style: {
          width: '160px',
        },
      },
    },
    {
      id: 'bank',
      label: 'Banco/Entidad',
      placeholder: 'Seleccione un banco',
      editable: false,
      type: InputType.Dropdown,
      endpoint: urls.banks.endpoint,
      mapper: {
        id: 'id',
        label: (record) => record.bank.name,
      },
    },
    {
      id: 'currency',
      label: 'Moneda',
      placeholder: 'Seleccione una moneda',
      editable: false,
      type: InputType.Dropdown,
      endpoint: urls.currencies.endpoint,
      mapper: {
        id: 'id',
        label: (record) => record.currency.name,
      },
    },
    {
      id: 'amount',
      label: 'Monto',
      placeholder: 'Monto',
      type: InputType.Decimal,
      min: 0.0,
      header: numericHeader,
      class: 'text-end',
      editable: {
        defaultValue: 0.0,
      },
      mapper: (record) => {
        console.log('Mapping amount for record:', record);
        return valueMapper(record.amount);
      },
      conditionalClass: valueConditionalClass,
    },
  ];

  const paginatedData = Array.isArray(data)
    ? {
        items: data,
        totalPages: 1,
      }
    : data;

  return (
    <div className={cn(['py-10', 'px-40'])}>
      <BankCurrencySelector
        banks={banks}
        currencies={currencies}
        bankId={bankId}
        currencyId={currencyId}
        onBankChange={onBankPickerChange}
        onCurrencyChange={onCurrencyPickerChange}
      />
      <PaginatedTable<IncomeRecord>
        name="incomes-table"
        columns={TableColumns}
        data={paginatedData}
        onAdd={() => reload({})}
        onDelete={() => reload({})}
        admin={{
          endpoint: urls.incomes.endpoint,
          key: [
            {
              id: 'BankId',
              value: bankId,
            },
            {
              id: 'CurrencyId',
              value: currencyId,
            },
          ],
        }}
      />
    </div>
  );
};

export default Incomes;
