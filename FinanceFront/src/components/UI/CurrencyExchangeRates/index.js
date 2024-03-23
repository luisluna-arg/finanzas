import React, { useEffect, useState } from "react";
import CommonUtils from "../../../utils/common";
import urls from "../../../routing/urls";
import Picker from "../../utils/Picker";
import PaginatedTable from "../../utils/PaginatedTable";
import { InputControlTypes } from "../../utils/InputControl";

function CurrencyExchangeRates() {
    const [selectedBaseCurrencyId, setSelectedBaseCurrencyId] = useState(null);
    const [selectedQuoteCurrencyId, setSelectedQuoteCurrencyId] = useState(null);
    const [currenciesEndpoint, setCurrenciesEndpoint] = useState(`${urls.currencies.endpoint}`);
    const [CurrencyExchangeRatesEndpoint, setCurrencyExchangeRatesEndpoint] = useState(`${urls.currencyExchangeRates.endpoint}`);
    const [CurrencyExchangeRatesPaginatedEndpoint, setCurrencyExchangeRatesPaginatedEndpoint] = useState(`${urls.currencyExchangeRates.paginated}`);

    const refreshEndpoints = () => {
        if (selectedQuoteCurrencyId && selectedBaseCurrencyId) {
            const urlParams = CommonUtils.Params({
                BaseCurrencyId: selectedBaseCurrencyId,
                QuoteCurrencyId: selectedQuoteCurrencyId,
                DateTimeKind: "Local"
            });
            setCurrencyExchangeRatesEndpoint(`${urls.currencyExchangeRates.paginated}?${urlParams}`);
        }
    }

    const onBaseCurrencyPickerChange = (picker) => {
        setSelectedBaseCurrencyId(picker.value);
        refreshEndpoints();
    };

    const onBaseCurrencyPickerFetch = (data) => {
        setSelectedBaseCurrencyId(data.data[0].id);
        refreshEndpoints();
    };

    const onQuoteCurrencyPickerChange = (picker) => {
        setSelectedQuoteCurrencyId(picker.value);
        refreshEndpoints();
    };

    const onQuoteCurrencyPickerFetch = (data) => {
        setSelectedQuoteCurrencyId(data.data[0].id);
        refreshEndpoints();
    };

    const onFetchCurrencyExchangeRatesTable = (data) => {

    }

    useEffect(() => {
        refreshEndpoints();
    }, [
        selectedBaseCurrencyId,
        selectedQuoteCurrencyId,
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

    const floatMapper = (field) => parseFloat(field.toFixed(2));

    const floatConditionalClass = {
        class: "text-success fw-bold",
        eval: (field) => field > 0
    };

    const CurrencyExchangeRatesTableColumns = [
        {
            id: "timeStamp",
            label: "Fecha",
            placeholder,
            type: InputControlTypes.DateTime,
            editable: false,
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
            id: "baseCurrency",
            key: "baseCurrencyId",
            label: "Mon. Base",
            placeholder,
            type: InputControlTypes.Dropdown,
            endpoint: currenciesEndpoint,
            editable: true,
            style: textStyle,
            mapper: {
                id: "id",
                label: "name"
            }
        },
        {
            id: "quoteCurrency",
            key: "quoteCurrencyId",
            label: "Mon. Cotizada",
            placeholder,
            type: InputControlTypes.Dropdown,
            endpoint: currenciesEndpoint,
            editable: true,
            style: textStyle,
            mapper: {
                id: "id",
                label: "name"
            }
        },
        {
            id: "buyRate",
            label: "Compra",
            placeholder,
            headerClass: "text-end",
            class: "text-end",
            editable: true,
            style: numericStyle,
            mapper: floatMapper,
            conditionalClass: floatConditionalClass
        },
        {
            id: "sellRate",
            label: "Venta",
            placeholder,
            headerClass: "text-end",
            class: "text-end",
            editable: true,
            style: numericStyle,
            mapper: floatMapper,
            conditionalClass: floatConditionalClass
        },
    ];

    let enabled = CurrencyExchangeRatesEndpoint
        && currenciesEndpoint;

    return (
        <>
            <div className="container pt-3 pb-3">
                {!enabled && <div>Cargando...</div>}
                {enabled && <div>
                    <PaginatedTable
                        name={"currency-rates-table"}
                        url={CurrencyExchangeRatesPaginatedEndpoint}
                        admin={{
                            endpoint: CurrencyExchangeRatesEndpoint,
                            key: [
                                {
                                    id: "QuoteCurrencyId",
                                    value: selectedQuoteCurrencyId
                                },
                                {
                                    id: "BaseCurrencyId",
                                    value: selectedBaseCurrencyId
                                }
                            ]
                        }}
                        onFetch={onFetchCurrencyExchangeRatesTable}
                        columns={CurrencyExchangeRatesTableColumns} />
                </div>
                }
            </div>
        </>
    );
}

export default CurrencyExchangeRates;
