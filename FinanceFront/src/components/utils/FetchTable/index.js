import React, { useState, useEffect } from 'react';
import { InputControlTypes } from "../../utils/InputControl";
import dates from "../../../utils/dates";

const FetchTable = ({ name, classes, title, url, columns, onFetch }) => {
    const [data, setData] = useState([]);
    const [isFetching, setIsFetching] = useState(false);

    const fetchData = async (dataUrl) => {
        setData([]);
        try {
            setIsFetching(true);
            if (dataUrl) {
                let fetchData = await fetch(dataUrl);
                let newData = await fetchData.json();
                setData(newData);
                onFetch && onFetch(newData);
            }
        } catch (error) {
            console.error("Error fetching data:", error);
        } finally {
            setIsFetching(false);
        }
    };

    useEffect(() => {
        if (url) {
            fetchData(url);
        }
    }, [url]);

    return (
        <>
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
                    {data && data.map((record) => {
                        return (
                            <tr key={record.id}>
                                {columns && columns.map((column, index) => {
                                    const value = column.mapper ? column.mapper(record[column.id]) : record[column.id];
                                    const displayValue = column.type && column.type === InputControlTypes.DateTime ? dates.toDisplay(value) : value;
                                    const useConditionalClass = column.conditionalClass && column.conditionalClass.eval(record[column.id]);
                                    const cssClasses = useConditionalClass ? column.conditionalClass.class : "";

                                    return (<td className={column.class} key={`${name}-${column.id}-${index}`}>
                                        <span className={cssClasses}>{displayValue}</span>
                                    </td>);
                                })}
                            </tr>
                        );
                    })}
                    {
                        (!data || data.length === 0) &&
                        <tr>
                            <td colSpan={columns.length}>
                                <div className='text-center'>No hay datos disponibles</div>
                            </td>
                        </tr>
                    }
                </tbody>
                {data && data.length > 0 && columns.filter(o => o.totals != null).length > 0 && (
                    <tfoot style={{ borderTop: "2px solid" }}>
                        <tr key={`${name}-totals-row`}>
                            {columns && columns.map((column, index) => {
                                const cellKey = `${name}-total-${column.id}-${index}`;
                                const isNumericCol = [InputControlTypes.Decimal, InputControlTypes.Integer].indexOf(column.type) > -1;

                                if (!column.totals && !isNumericCol) return <td key={cellKey}></td>

                                let value = data.reduce((acc, curr) => acc + column.totals.reducer(curr[column.id]), 0);
                                const useConditionalClass = column.conditionalClass && column.conditionalClass.eval(value);
                                const cssClasses = useConditionalClass ? column.conditionalClass.class : "";

                                return (<td className={column.class} key={cellKey}>
                                    <span className={cssClasses}>{value}</span>
                                </td>);
                            })}
                        </tr>
                    </tfoot>
                )}

            </table>
        </>
    );
};

export default FetchTable;
