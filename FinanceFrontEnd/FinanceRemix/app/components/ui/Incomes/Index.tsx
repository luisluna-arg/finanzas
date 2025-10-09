import React from "react";
import { useLoaderData, useLocation, useNavigate } from "@remix-run/react";
import urls from "@/utils/urls";
import dayjs from "dayjs";
import CommonUtils, { toNumber } from "@/utils/common";

import { InputType } from "@/components/ui/utils/InputType";
import Picker from "@/components/ui/utils/Picker";
import PaginatedTable, {
    Column,
    ConditionalClass,
} from "@/components/ui/utils/PaginatedTable";
import { cn } from "@/lib/utils";

// Define types for the props and states
interface PickerData {
    id: string;
    name: string;
}

interface LoaderData {
    banks: PickerData[];
    currencies: PickerData[];
    data: any[];
    bankId: string;
    currencyId: string;
}

const dateFormat = "DD/MM/YYYY";

const Incomes: React.FC = () => {
    const { banks, currencies, data, bankId, currencyId } =
        useLoaderData<LoaderData>();
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
        class: "text-success fw-bold",
        eval: (field: any) => field != null && toNumber(field) > 0,
    };

    const valueMapper = (field: any) =>
        field != null ? toNumber(field) : null;

    const numericHeader = {
        classes: "text-end",
        style: {
            width: "180px",
        },
    };

    const TableColumns: Column[] = [
        {
            id: "createdAt",
            label: "Fecha",
            placeholder: "Fecha",
            type: InputType.DateTime,
            editable: {
                defaultValue: () => dayjs().format(dateFormat),
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
                label: (record: PickerData) => `${record.name}`,
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
                label: (record: PickerData) => `${record.name}`,
            },
        },
        {
            id: "amount",
            label: "Monto",
            placeholder: "Monto",
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
    ];

    return (
        <div className={cn(["py-10", "px-40"])}>
            <div className="flex flex-row justify-center gap-10">
                <Picker
                    id="bank-picker"
                    value={bankId}
                    data={banks}
                    mapper={{ id: "id", label: (record) => `${record.name}` }}
                    onChange={onBankPickerChange}
                    className={"w-60"}
                />
                <Picker
                    id="currency-picker"
                    value={currencyId}
                    data={currencies}
                    mapper={{ id: "id", label: (record) => `${record.name}` }}
                    onChange={onCurrencyPickerChange}
                    className={"w-60"}
                />
            </div>
            <hr className={cn("py-1", "mb-5")} />
            <PaginatedTable
                name="incomes-table"
                columns={TableColumns}
                data={data}
                onAdd={() => reload({})}
                onDelete={() => reload({})}
                admin={{
                    endpoint: urls.incomes.endpoint,
                    key: [
                        {
                            id: "BankId",
                            value: bankId,
                        },
                        {
                            id: "CurrencyId",
                            value: currencyId,
                        },
                    ],
                }}
            />
        </div>
    );
};

export default Incomes;
