import React, { useState, useEffect, useCallback } from "react";

// Define types for the props
type MapperObject = {
  id: ((record: any) => string | number) | string;
  label: ((record: any) => string) | string;
};

// Define the Mapper type to be either a function or an object
type MapperType = ((record: any) => string | number) | MapperObject;

interface PickerProps {
  id: string;
  value?: string | number;
  url?: string;
  data?: any[];
  mapper: MapperType;
  onChange?: ((event: { value: string | number }) => void) | ((picker: { value: string; }) => void);
  onFetch?: any;
}

const Picker: React.FC<PickerProps> = ({ id, value, url, data, mapper, onChange, onFetch }) => {
  const [internalData, setInternalData] = useState<any[]>(data ?? []);

  const getRecordId = (record: any): string | number => {
    if (typeof mapper === "function") {
      return mapper(record);
    }

    if (typeof mapper.id === "function") {
      return mapper.id(record);
    }

    return record[mapper.id];
  };

  const getRecordLabel = (record: any): string => {
    if (typeof mapper === "function") {
      return String(mapper(record)); // If mapper is a function, return the result directly
    }

    if (typeof mapper.label === "function") {
      return mapper.label(record);
    }

    return record[mapper.label as string];
  };

  if (url) {
    const fetchData = useCallback(async () => {
      try {
        const response = await fetch(url);
        const responseData: any = await response.json();
        setInternalData(responseData);
        onFetch && onFetch({ responseData });
      } catch (error) {
        console.error("Error fetching data:", error);
      }
    }, [url, onFetch]);

    useEffect(() => {
      fetchData();
    }, [fetchData]);
  }
  
  const onPickerChange = (event: React.ChangeEvent<HTMLSelectElement>) => {
    onChange && onChange({ value: event.target.value });
  };
  
  return (
    <select id={id} className="form-select" onChange={onPickerChange} defaultValue={value}>
      {internalData.map((record) => {
        const recordId = getRecordId(record);
        const label = getRecordLabel(record);
        const key = `${id}-${recordId}`;

        return (
          <option key={key} value={recordId}>
            {label}
          </option>
        );
      })}
    </select>
  );
};

export default Picker;
