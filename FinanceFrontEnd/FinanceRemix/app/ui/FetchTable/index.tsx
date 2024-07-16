import React, { useState, useEffect, FunctionComponent } from 'react';
import { InputControlTypes } from "../InputControl";
import dates from "../../components/utils/dates";

interface FetchTableProps {
    name: string,
    classes: string,
    wrapper: any,
    title: any,
    data: any[],
    url: string,
    columns: any[],
    hideIfEmpty: string,
    onFetch: (data: any[]) => {},
    showTotals: boolean
}

const FetchTable: FunctionComponent<FetchTableProps> = ({
    name,
    classes,
    wrapper,
    title,
    data,
    url,
    columns,
    hideIfEmpty,
    onFetch,
    showTotals = true
    }) => {
    const [internalData, setInternalData] = useState(data ?? []);

    const fetchData = async (dataUrl: string) => {
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
                    {columns.map((column: any, index: number) => (
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
                            {columns && columns.map((column, index) => {
                                const value = column.mapper ? column.mapper(record) : record[column.id];
                                const useConditionalClass = column.conditionalClass && column.conditionalClass.eval(value);
                                const cssClasses = useConditionalClass ? column.conditionalClass.class : "";
                                let displayValue = column.type && column.type === InputControlTypes.DateTime ? dates.toDisplay(value) : value;
                                if (column.formatter) displayValue = column.formatter(displayValue);
                                return (<td className={column.class} key={`${name}-${column.id}-${index}`}>
                                    <span className={cssClasses}>{displayValue}</span>
                                </td>);
                            })}
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

                            if (!column.totals && !isNumericCol) return <td key={cellKey} style={{ borderTop: "3px solid" }}></td>

                            let value = internalData.reduce((acc, curr) => acc + column.totals.reducer(curr), 0);
                            const useConditionalClass = column.conditionalClass && column.conditionalClass.eval(value);
                            const cssClasses = useConditionalClass ? column.conditionalClass.class : "";

                            if (column.formatter) value = column.formatter(value);

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
