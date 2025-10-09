import { cn } from "@/lib/utils";
import React, { useState, useEffect, useCallback } from "react";
import SafeLogger from "@/utils/SafeLogger";

// Define types for the props
type MapperObject = {
    id: ((record: unknown) => string | number) | string;
    label: ((record: unknown) => string) | string;
};

// Define the Mapper type to be either a function or an object
export type MapperType = ((record: unknown) => string | number) | MapperObject;

interface PickerProps {
    id: string;
    value?: string | number;
    url?: string;
    data?: unknown[];
    mapper?: MapperType;
    onChange?:
        | ((event: { value: string | number }) => void)
        | ((picker: { value: string }) => void);
    onFetch?: (payload: { responseData: unknown }) => void;
    className?: string;
}

const Picker: React.FC<PickerProps> = ({
    id,
    value,
    url,
    data,
    mapper,
    className,
    onChange,
    onFetch,
}) => {
    const [internalData, setInternalData] = useState<unknown[]>(data ?? []);

    const getRecordId = (record: unknown): string | number => {
        if (typeof mapper === "function") {
            return mapper(record);
        }

        if (mapper && typeof (mapper as MapperObject).id === "function") {
            const fn = (mapper as MapperObject).id as (
                record: unknown
            ) => string | number;
            return fn(record);
        }

        if (typeof record === "object" && record !== null) {
            const key = (mapper as MapperObject).id as string | undefined;
            return String((record as Record<string, unknown>)[key ?? ""] ?? "");
        }

        return "";
    };

    const getRecordLabel = (record: unknown): string => {
        if (typeof mapper === "function") {
            return String(mapper(record)); // If mapper is a function, return the result directly
        }

        if (mapper && typeof (mapper as MapperObject).label === "function") {
            const fn = (mapper as MapperObject).label as (
                record: unknown
            ) => string;
            return fn(record);
        }

        if (typeof record === "object" && record !== null) {
            const key = (mapper as MapperObject).label as string | undefined;
            return String((record as Record<string, unknown>)[key ?? ""] ?? "");
        }

        return "";
    };
    const fetchData = useCallback(async () => {
        if (!url) return;
        try {
            const response = await fetch(url);
            const responseData: unknown = await response.json();
            if (Array.isArray(responseData)) {
                setInternalData(responseData);
            }
            onFetch && onFetch({ responseData });
        } catch (error) {
            SafeLogger.error("Error fetching data:", error);
        }
    }, [url, onFetch]);

    useEffect(() => {
        fetchData();
    }, [fetchData]);

    const onPickerChange = (event: React.ChangeEvent<HTMLSelectElement>) => {
        onChange && onChange({ value: event.target.value });
    };

    return (
        <select
            id={id}
            className={cn("form-select", className)}
            onChange={onPickerChange}
            defaultValue={value}
        >
            {internalData.map((record) => {
                const recordId = getRecordId(record);
                const label = getRecordLabel(record);
                const key = `${id}-${recordId}`;

                return (
                    <option key={key} value={String(recordId)}>
                        {label}
                    </option>
                );
            })}
        </select>
    );
};

export default Picker;
