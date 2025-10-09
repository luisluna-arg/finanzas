import React, { useEffect, useState } from "react";
import SafeLogger from "../../../utils/SafeLogger";
import dates from "@/utils/dates";
import LoadingSpinner from "@/components/ui/utils/LoadingSpinner";
import { InputType } from "@/components/ui/utils/InputType";
import {
    Table,
    TableBody,
    TableCell,
    TableFooter,
    TableHead,
    TableHeader,
    TableRow,
} from "@/components/ui/shadcn/table";

type FetchTableProps = {
    name?: string;
    url?: string;
    columns: any[];
    classes?: string[];
    wrapper?: { classes?: string[] };
    title?: { text: string; class: string };
    hideIfEmpty?: boolean;
    onFetch?: (newData: any[]) => void;
    showTotals?: boolean;
};

type FetchTableRowProps = {
    record: any;
    columns: any[];
};

const FetchTable: React.FC<FetchTableProps> = ({
    name,
    classes,
    wrapper,
    title,
    url,
    columns,
    showTotals = true,
}: FetchTableProps) => {
    const [data, setData] = useState<any>(null); // Adjust type as needed
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchData = async () => {
            setLoading(true);
            try {
                const response = await fetch(url!, {
                    credentials: "same-origin",
                });
                if (!response.ok) {
                    const errorText = `FetchTable fetch error: ${response.status} ${response.statusText}`;
                    let responseBody = "";
                    try {
                        responseBody = await response.text();
                        SafeLogger.log(responseBody);
                    } catch (e) {
                        responseBody = "[Unable to read response body]";
                    }
                    SafeLogger.error(errorText, {
                        url,
                        response: { status: response.status },
                        responseBody,
                    });
                    throw new Error(`${errorText}\n${responseBody}`);
                }
                const result = await response.json();

                setData(result.items ?? result);
            } catch (error: any) {
                SafeLogger.error("FetchTable error", {
                    error: error?.message ?? String(error),
                    url,
                });
                setError(error.message);
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, [url]);

    if (loading)
        return (
            <div>
                <LoadingSpinner />
            </div>
        );
    if (error) return <div>Error: {error}</div>;

    const getColumnValue = (columnSettings: any, record: any) => {
        const columnId = columnSettings.id ?? columnSettings.key;
        const mapper = columnSettings.mapper;
        const recordValue = record[columnId];

        if (mapper) {
            if (typeof mapper === "function") {
                return mapper(record);
            }
            if (Object.hasOwn(mapper, "label")) {
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

    const NoDataRow: React.FC<FetchTableProps> = ({
        columns,
    }: FetchTableProps) => {
        return (
            <TableRow>
                <TableCell colSpan={columns.length}>
                    <div>No hay datos disponibles</div>
                </TableCell>
            </TableRow>
        );
    };

    const DataRow: React.FC<FetchTableRowProps> = ({
        record,
        columns,
    }: FetchTableRowProps) => {
        return (
            <TableRow>
                {columns.map((column, index) => {
                    const columnValue = getColumnValue(column, record);
                    const cssClasses = getConditionalClasses(column, record);
                    let displayValue =
                        column.type && column.type === InputType.DateTime
                            ? dates.toDisplay(columnValue)
                            : columnValue;

                    if (column.formatter)
                        displayValue = column.formatter(displayValue);

                    return (
                        <TableCell
                            className={column.class}
                            key={`${name}-${column.id}-${index}`}
                        >
                            <span className={cssClasses}>{displayValue}</span>
                        </TableCell>
                    );
                })}
            </TableRow>
        );
    };

    const Content = () => (
        <Table>
            <TableHeader id={name} className={[...(classes ?? [])].join(" ")}>
                {title && (
                    <TableRow>
                        <TableHead
                            colSpan={columns.length}
                            className={title.class}
                        >
                            {title.text}
                        </TableHead>
                    </TableRow>
                )}
                <TableRow>
                    {columns.map((column, index) => (
                        <TableHead
                            className={column.headerClass}
                            scope="col"
                            key={index}
                            style={column?.header?.style ?? {}}
                        >
                            {column.label}
                        </TableHead>
                    ))}
                </TableRow>
            </TableHeader>
            <TableBody>
                {data.map((record: any) => (
                    <DataRow
                        key={record.id}
                        record={record}
                        columns={columns}
                    />
                ))}
                {(!data || data.length === 0) && (
                    <NoDataRow columns={columns} />
                )}
            </TableBody>
            {showTotals &&
                data.length > 0 &&
                columns.some((column) => column.totals) && (
                    <TableFooter>
                        <TableRow key={`${name}-totals-row`}>
                            {columns.map((column, index) => {
                                const cellKey = `${name}-total-${column.id}-${index}`;
                                const isNumericCol = [
                                    InputType.Decimal,
                                    InputType.Integer,
                                ].includes(column.type);

                                if (!isNumericCol)
                                    return (
                                        <TableCell
                                            key={cellKey}
                                            style={{ borderTop: "3px solid" }}
                                        />
                                    );

                                if (!column?.totals?.reducer) {
                                    throw new Error(
                                        `Totals reducer for table "${name}"'s colum "${column.id}" undefined`
                                    );
                                }

                                let value = data.reduce(
                                    column?.totals?.reducer,
                                    0
                                );

                                const useConditionalClass =
                                    column.conditionalClass &&
                                    column.conditionalClass.eval(value);
                                const cssClasses = useConditionalClass
                                    ? column.conditionalClass.class
                                    : "";

                                if (
                                    column?.totals?.formatter &&
                                    typeof column?.totals?.formatter ===
                                        "function"
                                ) {
                                    value = column.totals.formatter(value);
                                }

                                return (
                                    <TableCell
                                        className={column.class}
                                        style={{ borderTop: "3px solid" }}
                                        key={cellKey}
                                    >
                                        <span className={cssClasses}>
                                            {value}
                                        </span>
                                    </TableCell>
                                );
                            })}
                        </TableRow>
                    </TableFooter>
                )}
        </Table>
    );

    return wrapper ? (
        <div className={wrapper.classes?.join(" ")}>
            <Content />
        </div>
    ) : (
        <Content />
    );
};

export default FetchTable;
