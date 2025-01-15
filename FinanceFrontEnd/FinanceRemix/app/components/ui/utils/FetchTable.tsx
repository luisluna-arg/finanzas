import React, { useEffect, useState } from 'react';
import dates from "@/app/utils/dates";
import LoadingSpinner from "@/app/components/ui/utils/LoadingSpinner";
import { InputType } from "@/app/components/ui/utils/InputType";

type FetchTableProps = {
    name: string;
    url: string;
    columns: any[];
    classes?: string[];
    wrapper?: { classes?: string[] };
    title?: { text: string; class: string };
    hideIfEmpty?: boolean;
    onFetch?: (newData: any[]) => void;
    showTotals?: boolean;
};

const FetchTable: React.FC<FetchTableProps> = ({
    name,
    classes,
    wrapper,
    title,
    url,
    columns,
    hideIfEmpty,
    onFetch,
    showTotals = true,
}: FetchTableProps) => {
    const [data, setData] = useState<any>(null); // Adjust type as needed
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchData = async () => {
            setLoading(true);
            try {
                const response = await fetch(url);
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                const result = await response.json();
                setData(result.items ?? result);
            } catch (error: any) {
                setError(error.message);
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, [url]);

    if (loading) return <div><LoadingSpinner /></div>;
    if (error) return <div>Error: {error}</div>;

    const getColumnValue = (columnSettings: any, record: any) => {
        const columnId = columnSettings.id ?? columnSettings.key;
        const mapper = columnSettings.mapper;
        const recordValue = record[columnId];

        if (mapper) {
            if (typeof mapper === "function") {
                return mapper(record);
            }
            if (mapper.hasOwnProperty("label")) {
                return recordValue[mapper["label"]];
            }
        }

        return recordValue;
    };

    const getConditionalClasses = (column: any, record: any) => {
        if (!column.conditionalClass) return [];

        const value = getColumnValue(column, record);
        const rules = Array.isArray(column.conditionalClass)
            ? column.conditionalClass
            : [column.conditionalClass];

        return rules
            .filter((conditional: any) => conditional.eval(value))
            .map((conditional: any) => conditional.class)
            .join(" ");
    };

    const rowColumnMapper = (record: any, columns: any[]) => {
        return columns.map((column, index) => {
            const columnValue = getColumnValue(column, record);
            const cssClasses = getConditionalClasses(column, record);
            let displayValue =
                column.type && column.type === InputType.DateTime
                    ? dates.toDisplay(columnValue)
                    : columnValue;

            if (column.formatter) displayValue = column.formatter(displayValue);

            return (
                <td className={column.class} key={`${name}-${column.id}-${index}`}>
                    <span className={cssClasses}>{displayValue}</span>
                </td>
            );
        });
    };

    const Content = () => (
        <table id={name} className={["table", ...(classes ?? [])].join(" ")}>
            <thead>
                {title && (
                    <tr>
                        <th colSpan={columns.length} className={title.class}>
                            {title.text}
                        </th>
                    </tr>
                )}
                <tr>
                    {columns.map((column, index) => (
                        <th className={column.headerClass} scope="col" key={index} style={column?.header?.style ?? {}}>
                            {column.label}
                        </th>
                    ))}
                </tr>
            </thead>
            <tbody>
                {data.map((record: any) => (
                    <tr key={record.id}>{rowColumnMapper(record, columns)}</tr>
                ))}
                {(!data || data.length === 0) && (
                    <tr>
                        <td colSpan={columns.length}>
                            <div className="text-center">No hay datos disponibles</div>
                        </td>
                    </tr>
                )}
            </tbody>
            {showTotals &&
                data.length > 0 &&
                columns.some((column) => column.totals) && (
                    <tfoot>
                        <tr key={`${name}-totals-row`}>
                            {columns.map((column, index) => {
                                const cellKey = `${name}-total-${column.id}-${index}`;
                                const isNumericCol = [
                                    InputType.Decimal,
                                    InputType.Integer,
                                ].includes(column.type);

                                if (!isNumericCol) return <td key={cellKey} style={{ borderTop: "3px solid" }}></td>;

                                if (!column?.totals?.reducer) {
                                    throw new Error(`Totals reducer for table "${name}"'s colum "${column.id}" undefined`);
                                }

                                let value = data.reduce(column?.totals?.reducer, 0);

                                const useConditionalClass =
                                    column.conditionalClass &&
                                    column.conditionalClass.eval(value);
                                const cssClasses = useConditionalClass
                                    ? column.conditionalClass.class
                                    : "";

                                if (column?.totals?.formatter && typeof column?.totals?.formatter === "function") {
                                    value = column.totals.formatter(value);
                                }

                                return (
                                    <td className={column.class} style={{ borderTop: "3px solid" }} key={cellKey}>
                                        <span className={cssClasses}>{value}</span>
                                    </td>
                                );
                            })}
                        </tr>
                    </tfoot>
                )}
        </table>
    );

    return wrapper ? (
        <div className={wrapper.classes?.join(" ")}>
            <Content />
        </div>
    ) : (
        <Content />
    );
}

export default FetchTable;

