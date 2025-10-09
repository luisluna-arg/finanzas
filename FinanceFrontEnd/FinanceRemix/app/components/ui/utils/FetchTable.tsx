/* eslint-disable react/prop-types */
import React, { useEffect, useState } from "react";
import SafeLogger from "../../../utils/SafeLogger";
import dates from "@/utils/dates";
import LoadingSpinner from "@/components/ui/utils/LoadingSpinner";
import { InputType } from "@/components/ui/utils/InputType";
import {
    Table,
    TableHeader,
    TableBody,
    TableFooter,
    TableHead,
    TableRow,
    TableCell,
} from "@/components/ui/shadcn/table";

// Local types (kept here to make the component self-contained)
type Row = { id?: string | number; [key: string]: unknown };

interface ConditionalClass {
    eval: (value: unknown) => boolean;
    class: string;
}

interface ColumnTotals {
    reducer?: (acc: unknown, row: Row) => unknown;
    formatter?: (value: unknown) => unknown;
}

interface Column {
    id?: string;
    key?: string;
    type?: InputType;
    class?: string;
    label?: string;
    headerClass?: string;
    header?: { style?: React.CSSProperties };
    formatter?: (value: unknown) => unknown;
    mapper?: { label?: string } | ((r: Row) => unknown);
    conditionalClass?: ConditionalClass | ConditionalClass[];
    totals?: ColumnTotals;
}

interface FetchTableProps {
    name: string;
    data?: Row[] | null;
    url?: string;
    columns: unknown[];
    classes?: string[];
    wrapper?: { classes?: string[] } | null;
    title?: { text: React.ReactNode; class?: string } | null;
    hideIfEmpty?: boolean;
    onFetch?: (data: Row[]) => void;
    showTotals?: boolean;
}

interface FetchTableRowProps {
    record: Row;
    columns: unknown[];
}

const FetchTable: React.FC<FetchTableProps> = ({
    name,
    classes,
    wrapper,
    title,
    url,
    data: initialData,
    columns,
    onFetch,
    showTotals = true,
}) => {
    const [data, setData] = useState<Row[] | null>(initialData ?? null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        if (!url) return;
        const fetchData = async () => {
            setLoading(true);
            try {
                const response = await fetch(url);
                if (!response.ok) {
                    const errorText = `FetchTable request failed for ${url} with status ${response.status}`;
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

                if (result && Array.isArray(result.items)) {
                    setData(result.items as Row[]);
                    onFetch && onFetch(result.items as Row[]);
                } else if (Array.isArray(result)) {
                    setData(result as Row[]);
                    onFetch && onFetch(result as Row[]);
                } else {
                    setData([]);
                    onFetch && onFetch([]);
                }
            } catch (err: unknown) {
                const msg = err instanceof Error ? err.message : String(err);
                SafeLogger.error("FetchTable error", {
                    error: msg,
                    url,
                });
                setError(msg);
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, [url, onFetch]);

    if (loading)
        return (
            <div>
                <LoadingSpinner />
            </div>
        );
    if (error) return <div>Error: {error}</div>;

    const getColumnValue = (columnSettings: unknown, record: Row) => {
        const column = columnSettings as Column;
        const columnId = column.id ?? column.key ?? "";
        const mapper = column.mapper;
        const recordValue = (record as Record<string, unknown>)[columnId];

        if (mapper) {
            if (typeof mapper === "function") {
                return (mapper as (r: Row) => unknown)(record);
            }
            if (Object.hasOwn(mapper, "label")) {
                const label = (mapper as { label?: string }).label;
                return label
                    ? (record as Record<string, unknown>)[String(label)]
                    : undefined;
            }
        }

        return recordValue;
    };

    const getConditionalClasses = (columnSetting: unknown, record: Row) => {
        const column = columnSetting as Column;
        if (!column.conditionalClass) return "";

        const value = getColumnValue(column, record);
        const rules = Array.isArray(column.conditionalClass)
            ? column.conditionalClass
            : [column.conditionalClass];

        return rules
            .filter((conditional) => conditional.eval(value))
            .map((conditional) => conditional.class)
            .join(" ");
    };

    const NoDataRow: React.FC<{ columns: unknown[] }> = ({ columns }) => {
        return (
            <TableRow>
                <TableCell colSpan={columns.length}>
                    <div>No hay datos disponibles</div>
                </TableCell>
            </TableRow>
        );
    };

    const DataRow: React.FC<FetchTableRowProps> = ({ record, columns }) => {
        return (
            <TableRow>
                {columns.map((col, index) => {
                    const column = col as Column;
                    const columnValue = getColumnValue(column, record);
                    const cssClasses = getConditionalClasses(column, record);
                    let displayValue =
                        column.type && column.type === InputType.DateTime
                            ? dates.toDisplay(String(columnValue))
                            : columnValue;

                    if (column.formatter)
                        displayValue = (
                            column.formatter as (v: unknown) => unknown
                        )(displayValue);

                    return (
                        <TableCell
                            className={column.class}
                            key={`${name}-${column.id}-${index}`}
                        >
                            <span className={cssClasses}>
                                {displayValue == null
                                    ? ""
                                    : String(displayValue)}
                            </span>
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
                    {columns.map((col, index) => {
                        const column = col as Column;
                        return (
                            <TableHead
                                className={column.headerClass}
                                scope="col"
                                key={index}
                                style={column?.header?.style ?? {}}
                            >
                                {column.label}
                            </TableHead>
                        );
                    })}
                </TableRow>
            </TableHeader>
            <TableBody>
                {data?.map((record: Row, index: number) => (
                    <DataRow
                        key={String(
                            (record as Record<string, unknown>).id ?? index
                        )}
                        record={record}
                        columns={columns}
                    />
                ))}
                {(!data || data.length === 0) && (
                    <NoDataRow columns={columns} />
                )}
            </TableBody>
            {showTotals &&
                data &&
                data.length > 0 &&
                columns.some((c) => (c as Column).totals) && (
                    <TableFooter>
                        <TableRow key={`${name}-totals-row`}>
                            {columns.map((col, index) => {
                                const column = col as Column;
                                const cellKey = `${name}-total-${column.id}-${index}`;
                                const isNumericCol =
                                    column.type !== undefined &&
                                    [
                                        InputType.Decimal,
                                        InputType.Integer,
                                    ].includes(column.type as InputType);

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

                                const value = (data ?? []).reduce(
                                    (acc, item) =>
                                        (
                                            column.totals!.reducer as (
                                                a: unknown,
                                                r: Row
                                            ) => unknown
                                        )(acc as unknown, item),
                                    0 as unknown
                                );

                                let cssClasses = "";
                                if (column.conditionalClass) {
                                    if (
                                        Array.isArray(column.conditionalClass)
                                    ) {
                                        const useConditional =
                                            column.conditionalClass.some((c) =>
                                                c.eval(value)
                                            );
                                        cssClasses = useConditional
                                            ? column.conditionalClass[0].class
                                            : "";
                                    } else {
                                        cssClasses =
                                            column.conditionalClass.eval(value)
                                                ? column.conditionalClass.class
                                                : "";
                                    }
                                }

                                let formattedValue: unknown = value;
                                if (
                                    column?.totals?.formatter &&
                                    typeof column.totals.formatter ===
                                        "function"
                                ) {
                                    formattedValue =
                                        column.totals.formatter(value);
                                }

                                return (
                                    <TableCell
                                        className={column.class}
                                        style={{ borderTop: "3px solid" }}
                                        key={cellKey}
                                    >
                                        <span className={cssClasses}>
                                            {String(formattedValue)}
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
