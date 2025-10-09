import React, { useState } from "react";
import { useLoaderData } from "@remix-run/react";
import dayjs from "dayjs";
import urls from "@/utils/urls";
import Picker from "@/components/ui/utils/Picker";
import PaginatedTable, { Column } from "@/components/ui/utils/PaginatedTable";
import { InputType } from "@/components/ui/utils/InputType";
import { toNumber } from "@/utils/common";

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
    return urls.proxy(urls.funds.paginated, {
        BankId: bankId,
        CurrencyId: currencyId,
        Page: 1,
        PageSize: 10,
    });
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

    // Removed updateFundsEndpoint helper to keep effect dependencies stable.

    const onBankPickerChange = (picker: { value: string }) => {
        const newBankId =
            selectedBankId !== picker.value ? picker.value : selectedBankId;
        setSelectedBankId(newBankId);
    };

    const onCurrencyPickerChange = (picker: { value: string }) => {
        const newCurrencyId =
            selectedCurrencyId !== picker.value
                ? picker.value
                : selectedCurrencyId;
        setSelectedCurrencyId(newCurrencyId);
    };

    const valueConditionalClass = {
        class: "text-success fw-bold",
        eval: (field: unknown) => field != null && toNumber(field) > 0,
    };

    const valueMapper = (field: unknown) =>
        field != null ? toNumber(field) : null;

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
                    const rowSelector = document.querySelector(
                        ".bank-table-data-row > td > span"
                    );

                    if (!rowSelector?.textContent)
                        return dayjs().format(dateFormat);

                    return dayjs(
                        rowSelector.textContent,
                        `${dateFormat}`
                    ).format(dateFormat);
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
            setFundsEndpoint(
                buildFundsEndpoint(selectedBankId, selectedCurrencyId)
            );
            setReloadTable(true);
        }
    }, [selectedBankId, selectedCurrencyId]);

    return (
        <>
            <div className="container pt-3 pb-3">
                <div className="row">
                    <div className="col-6">
                        <Picker
                            id={"bank-picker"}
                            data={banks}
                            mapper={{
                                id: "id",
                                label: (record: unknown) =>
                                    `${(record as PickerData).name}`,
                            }}
                            onChange={onBankPickerChange}
                            onFetch={({
                                responseData,
                            }: {
                                responseData: unknown;
                            }) => {
                                if (
                                    Array.isArray(responseData) &&
                                    responseData.length > 0
                                ) {
                                    const first = responseData[0] as PickerData;
                                    setSelectedBankId(first.id);
                                }
                            }}
                        />
                    </div>
                    <div className="col-6">
                        <Picker
                            id={"currency-picker"}
                            data={currencies}
                            mapper={{
                                id: "id",
                                label: (record: unknown) =>
                                    `${(record as PickerData).name}`,
                            }}
                            onChange={onCurrencyPickerChange}
                            onFetch={({
                                responseData,
                            }: {
                                responseData: unknown;
                            }) => {
                                if (
                                    Array.isArray(responseData) &&
                                    responseData.length > 0
                                ) {
                                    const first = responseData[0] as PickerData;
                                    setSelectedCurrencyId(first.id);
                                }
                            }}
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
