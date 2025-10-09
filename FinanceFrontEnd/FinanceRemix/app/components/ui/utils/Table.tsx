import React from "react";
import {
    Table as TableUI,
    TableBody,
    TableHead,
    TableHeader,
    TableRow,
    TableCell,
} from "@/components/ui/shadcn/table";

export interface TableColumn {
    id: string;
    label: string;
    mapper?: (record: unknown) => React.ReactNode;
    className?: string | Array<string>;
    headerClassName?: string | Array<string>;
}

export interface TableProps {
    data: Array<unknown>;
    columns: Array<TableColumn>;
}

function resolveClassName(
    headerClassName: string | string[] | undefined
): string {
    const arr = Array.isArray(headerClassName)
        ? (headerClassName as string[])
        : [headerClassName ?? ""];
    return arr.join(" ");
}

const Table: React.FC<TableProps> = ({ data, columns }) => {
    function resolveColumnValue(d: unknown, c: TableColumn): React.ReactNode {
        if (c.mapper) {
            return c.mapper(d);
        }

        // Try to read the property safely
        if (typeof d === "object" && d !== null) {
            const value = (d as Record<string, unknown>)[c.id];
            if (value === null || value === undefined) return null;
            // If it's already a React node, return it; otherwise stringify simple objects.
            if (React.isValidElement(value)) return value;
            if (typeof value === "object") return JSON.stringify(value);
            return String(value) as React.ReactNode;
        }
        return null;
    }

    return (
        <TableUI>
            <TableHeader>
                <TableRow>
                    {columns.map((c) => (
                        <TableHead
                            key={`${c.id}-head`}
                            className={resolveClassName(c.headerClassName)}
                        >
                            {c.label}
                        </TableHead>
                    ))}
                </TableRow>
            </TableHeader>
            <TableBody>
                {data.map((d: unknown, idx: number) => {
                    const key =
                        d &&
                        typeof d === "object" &&
                        "id" in (d as Record<string, unknown>)
                            ? String((d as Record<string, unknown>).id)
                            : idx.toString();
                    return (
                        <TableRow key={key}>
                            {columns.map((c: TableColumn) => (
                                <TableCell
                                    key={`${c.id}-body`}
                                    className={resolveClassName(c.className)}
                                >
                                    {resolveColumnValue(d, c)}
                                </TableCell>
                            ))}
                        </TableRow>
                    );
                })}
            </TableBody>
        </TableUI>
    );
};

export default Table;
