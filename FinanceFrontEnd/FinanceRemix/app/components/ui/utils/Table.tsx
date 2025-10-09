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
    mapper?: Function;
    className?: string | Array<string>;
    headerClassName?: string | Array<string>;
}

export interface TableProps {
    data: Array<any>;
    columns: Array<TableColumn>;
}

function resolveClassName(
    headerClassName: string | string[] | undefined
): string {
    return (
        typeof headerClassName == typeof Array<string>
            ? (headerClassName as Array<string>)
            : [headerClassName ?? ""]
    ).join(" ");
}

const Table: React.FC<TableProps> = ({ data, columns }) => {
    function resolveColumnValue(d: any, c: TableColumn): React.ReactNode {
        if (c.mapper) {
            return c.mapper(d);
        }

        return d[c.id];
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
                {data.map((d: any) => (
                    <TableRow key={d.id}>
                        {columns.map((c: TableColumn) => (
                            <TableCell
                                key={`${c.id}-body`}
                                className={resolveClassName(c.className)}
                            >
                                {resolveColumnValue(d, c)}
                            </TableCell>
                        ))}
                    </TableRow>
                ))}
            </TableBody>
        </TableUI>
    );
};

export default Table;
