import React, { useState, useEffect } from 'react';
import { InputControlTypes } from "../../utils/InputControl";
import dates from "../../../utils/dates";

const FetchTable = ({
    name,
    classes,
    wrapper,
    title, data,
    url,
    columns,
    hideIfEmpty,
    onFetch,
    showTotals = true
}) => {
    const [internalData, setInternalData] = useState(data ?? []);

    const fetchData = async (dataUrl) => {
        setInternalData([]);
        try {
            if (dataUrl) {
                let fetchData = await fetch(dataUrl);
                let newData = await fetchData.json();
                setInternalData(newData);
                onFetch && onFetch(newData);
            }
        } catch (error) {
            console.error("Error fetching data:", error);
        } finally {
        }
    };

    const getColumnValue = (columnSettings, record) => {
        const columnId = columnSettings.id ?? columnSettings.key;

        const mapper = columnSettings.mapper;

        var recordValue = record[columnId];

        console.info("");
        console.info("columnSettings", columnSettings);
        console.info("record", record);
        console.info("mapper", mapper);
        console.info("columnId", columnId);
        console.info("recordValue", recordValue);

        if (mapper) {
            if (typeof mapper == 'function') {
                console.info("mapper(recordValue)", mapper(recordValue));
                return mapper(recordValue);
            }

            if (mapper.hasOwnProperty("label")) {
                return recordValue[mapper["label"]];
            }
        }

        console.info("");

        return recordValue;
    }

    const getConditionalClasses = (column, record) => {
        if (!(column.conditionalClass)) {
            return [];
        }

        const value = getColumnValue(column, record);

        let evaluate = (evaluator, columnValue) => evaluator(columnValue);

        let rules = Array.isArray(column.conditionalClass) ? column.conditionalClass : [column.conditionalClass];

        var result = [];
        for (let i = 0; i < rules.length; i++) {
            const conditional = rules[i];

            if (evaluate(conditional.eval, value)) {
                result = result.concat(conditional.class.split(" "));
            }
        }

        return result.join(" ");
    };

    let rowColumnMapper = (record, columns) => {
        return columns.map((column, index) => {
            const columnValue = getColumnValue(column, record);
            const cssClasses = getConditionalClasses(column, record);
            let displayValue = column.type && column.type === InputControlTypes.DateTime ? dates.toDisplay(columnValue) : columnValue;
            if (column.formatter) displayValue = column.formatter(displayValue);
            return (<td className={column.class} key={`${name}-${column.id}-${index}`}>
                <span className={cssClasses}>{displayValue}</span>
            </td>);
        })
    };

    useEffect(() => {
        if (url) {
            fetchData(url);
        }
    }, [url]);

    if (hideIfEmpty && (!internalData || internalData.length === 0)) return (<></>);

    const Content = () => (
        <table id={name} className={["table", ...classes].reduce((prev, curr) => `${prev} ${curr}`)}>
            <thead>
                {title && <tr>
                    <th colSpan={columns.length}
                        className={title.class}>{title.text}</th>
                </tr>}
                <tr>
                    {columns.map((column, index) => (
                        <th className={column.headerClass} scope="col" key={index}>
                            {column.label}
                        </th>
                    ))}
                </tr>
            </thead>
            <tbody>
                {internalData && internalData.map((record) => {
                    return (
                        <tr key={record.id}>
                            {columns && rowColumnMapper(record, columns)}
                        </tr>
                    );
                })}
                {
                    (!internalData || internalData.length === 0) &&
                    <tr>
                        <td colSpan={columns.length}>
                            <div className='text-center'>No hay datos disponibles</div>
                        </td>
                    </tr>
                }
            </tbody>
            {showTotals && internalData && internalData.length > 0 && columns.filter(o => o.totals != null).length > 0 && (
                <tfoot>
                    <tr key={`${name}-totals-row`}>
                        {columns && columns.map((column, index) => {
                            const cellKey = `${name}-total-${column.id}-${index}`;
                            const isNumericCol = [InputControlTypes.Decimal, InputControlTypes.Integer].indexOf(column.type) > -1;

                            if (!isNumericCol) return <td key={cellKey} style={{ borderTop: "3px solid" }}></td>

                            if (!(column?.totals?.reducer)) {
                                throw new Error(`Totals reducer for table "${name}"'s colum "${column.id}" undefined`);
                            }

                            // console.log("table", name);
                            // console.log("column?.mapper", column?.mapper);

                            let value = 0;
                            if (column?.totals?.reducer) {
                                // console.log("column?.totals?.reducer");
                                // console.log("       ", column?.totals?.reducer);
                                // console.log("       ", internalData[0]);
                                value = internalData.reduce(column?.totals?.reducer, 0);
                            }
                            else {
                                console.log("internalData.reduce", internalData.reduce);
                                value = internalData.reduce((acc, r) => acc + (column?.mapper ?? (v => v))(r), 0)
                            }

                            const useConditionalClass = column.conditionalClass && column.conditionalClass.eval(value);
                            const cssClasses = useConditionalClass ? column.conditionalClass.class : "";

                            // console.log("       column", column?.id, column?.label, value);

                            if (column?.totals?.formatter && typeof column?.totals?.formatter === "function") {
                                // console.log("       column.totals.formatter(value)", column.totals.formatter(value));
                                value = column.totals.formatter(value);
                            }

                            return (<td className={column.class} style={{ borderTop: "3px solid" }} key={cellKey}>
                                <span className={cssClasses}>{value}</span>
                            </td>);
                        })}
                    </tr>
                </tfoot>
            )}
        </table>
    );

    if (wrapper) {
        let wrapperClasses = wrapper.classes ?? [];
        return (<div className={wrapperClasses.join(' ')}>
            <Content />
        </div>);
    }
    else {
        return (<Content />);
    }
};

export default FetchTable;
