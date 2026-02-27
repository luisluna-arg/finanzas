import { cn } from '@/lib/utils';
import React, { useState, useEffect, useCallback } from 'react';
import SafeLogger from '@/utils/SafeLogger';
import type { BackendUrl } from '@/utils/BackendUrl';

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
  url?: BackendUrl | string;
  data?: unknown[];
  mapper?: MapperType;
  onChange?: ((event: { value: string | number }) => void) | ((picker: { value: string }) => void);
  onFetch?: (payload: { responseData: unknown }) => void;
  className?: string;
  placeholder?: string;
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
  placeholder,
}) => {
  const [internalData, setInternalData] = useState<unknown[]>(data ?? []);

  const getRecordId = (record: unknown): string | number => {
    if (typeof mapper === 'function') {
      return mapper(record);
    }

    if (mapper && typeof (mapper as MapperObject).id === 'function') {
      const fn = (mapper as MapperObject).id as (record: unknown) => string | number;
      return fn(record);
    }

    if (typeof record === 'object' && record !== null) {
      const key = (mapper as MapperObject).id as string | undefined;
      return String((record as Record<string, unknown>)[key ?? ''] ?? '');
    }

    return '';
  };

  const getRecordLabel = (record: unknown): string => {
    if (typeof mapper === 'function') {
      return String(mapper(record)); // If mapper is a function, return the result directly
    }

    if (mapper && typeof (mapper as MapperObject).label === 'function') {
      const fn = (mapper as MapperObject).label as (record: unknown) => string;
      return fn(record);
    }

    if (typeof record === 'object' && record !== null) {
      const key = (mapper as MapperObject).label as string | undefined;
      return String((record as Record<string, unknown>)[key ?? ''] ?? '');
    }

    return '';
  };
  const fetchData = useCallback(async () => {
    if (!url) return;
    try {
      const response = await fetch(String(url));
      const responseData: unknown = await response.json();
      if (Array.isArray(responseData)) {
        setInternalData(responseData);
      }
      onFetch && onFetch({ responseData });
    } catch (error) {
      SafeLogger.error('Error fetching data:', error);
    }
  }, [url, onFetch]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  const onPickerChange = (value: string | null) => {
    if (value !== null) {
      onChange && onChange({ value });
    }
  };

  return (
    <select
      id={id}
      value={value?.toString() ?? ''}
      onChange={(e) => onPickerChange(e.target.value)}
      className={cn(
        'h-9 rounded-md border border-gray-300 bg-white px-3 py-1 text-sm text-gray-950 shadow-sm outline-none focus:border-gray-950 focus:ring-2 focus:ring-gray-950/20 dark:bg-gray-950 dark:text-gray-50 dark:border-gray-700',
        className
      )}
    >
      <option value="" disabled className="text-gray-500">
        {placeholder ?? 'Select an item'}
      </option>
      {internalData.map((item) => {
        const recordId = getRecordId(item);
        const label = getRecordLabel(item);
        const key = `${id}-${recordId}`;
        return (
          <option key={key} value={recordId} className="text-gray-950 dark:text-gray-50">
            {label}
          </option>
        );
      })}
    </select>
  );
};

export default Picker;
