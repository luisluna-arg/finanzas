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

    const updatePaginatedEndpoint = (baseCurrencyId, quoteCurrencyId) => {
        const params = CommonUtils.Params({
            BaseCurrencyId: baseCurrencyId,
            QuoteCurrencyId: quoteCurrencyId
        });
        setCurrencyExchangeRatesPaginatedEndpoint(`${urls.currencyExchangeRates.paginated}?${params}`);
    };

    const onBaseCurrencyPickerChange = (picker) => {
        var newBaseCurrencyId = selectedBaseCurrencyId !== picker.value ? picker.value : selectedBaseCurrencyId;
        setSelectedBaseCurrencyId(newBaseCurrencyId);
    };

    const onBaseCurrencyPickerFetch = ({ data }) => {
        var newBaseCurrencyId = data[0].id;
        setSelectedBaseCurrencyId(newBaseCurrencyId);
    };

    const onQuoteCurrencyPickerChange = (picker) => {
        var newQuoteCurrencyId = selectedQuoteCurrencyId !== picker.value ? picker.value : selectedQuoteCurrencyId;
        setSelectedQuoteCurrencyId(newQuoteCurrencyId);
    };

    const onQuoteCurrencyPickerFetch = ({ data }) => {
        var newQuoteCurrencyId = data[0].id;
        setSelectedQuoteCurrencyId(newQuoteCurrencyId);
    };

    const onFetchCurrencyExchangeRatesTable = (data) => {

    }

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
            editable: false,
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
            editable: false,
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

    useEffect(() => {
        if (selectedBaseCurrencyId && selectedQuoteCurrencyId) {
            updatePaginatedEndpoint(selectedBaseCurrencyId, selectedQuoteCurrencyId);
        }
    }, [selectedBaseCurrencyId, selectedQuoteCurrencyId]);

    return (
        <>
            <div className="container pt-3 pb-3">
                <div className="row">
                    <div className="col-6 row row-cols-3">
                        <div className="col-2 mt-2">
                            <span className="fw-bold">Base</span>
                        </div>
                        <div className="col-10">
                            <Picker
                                id={"base-currency-picker"}
                                url={urls.currencies.endpoint}
                                mapper={{ id: "id", label: record => `${record.name}` }}
                                onChange={onBaseCurrencyPickerChange}
                                onFetch={onBaseCurrencyPickerFetch}
                            />
                        </div>
                    </div>
                    <div className="col-6 row row-cols-3">
                        <div className="col-2 mt-2">
                            <span className="fw-bold">Cotización</span>
                        </div>
                        <div className="col-10">
                            <Picker
                                id={"quote-currency-picker"}
                                url={urls.currencies.endpoint}
                                mapper={{ id: "id", label: record => `${record.name}` }}
                                onChange={onQuoteCurrencyPickerChange}
                                onFetch={onQuoteCurrencyPickerFetch}
                            />
                        </div>
                    </div>
                </div>
                <hr className="py-1" />
                {CurrencyExchangeRatesPaginatedEndpoint && <div>
                    <PaginatedTable
                        name={"currency-rates-table"}
                        url={CurrencyExchangeRatesPaginatedEndpoint}
                        columns={CurrencyExchangeRatesTableColumns}
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
                    />
                </div>
                }
            </div>
        </>
    );
}

export default CurrencyExchangeRates;
