import React, { useState, useEffect, useCallback, FunctionComponent } from "react";

interface Mapper {
  label: string | ((record: any) => string);
  id?: string | ((record: any) => string);
}

interface PickerProps {
  id: string;
  value: string;
  url: string;
  mapper: Function | any;
  onChange?: (event: { value: string }) => void;
  onFetch?: (data: { data: any[] }) => void;
}

const Picker: FunctionComponent<PickerProps> = ({ id, value, url, mapper, onChange, onFetch }) => {
  const [data, setData] = useState<any[]>([]);

  const getRecordId = (record: any): string => {
    let recordIdField = "id";

    if (mapper) {
      if (typeof mapper === "function") recordIdField = mapper(record);

      if (mapper.id) {
        if (typeof mapper.id === "function") recordIdField = mapper.id(record);
        else recordIdField = mapper.id;
      }
    }

    return record[recordIdField];
  };

  const getRecordLabel = (record: any): string => {
    return typeof mapper.label === "function" ? mapper.label(record) : record[mapper.label];
  };

  const fetchData = useCallback(async () => {
    try {
      const data = await (await fetch(url)).json();
      setData(data);
      onFetch && onFetch({ data });
    } catch (error) {
      console.error("Error fetching data:", error);
    }
  }, [url, onFetch]);

  const onPickerChange = (event: React.ChangeEvent<HTMLSelectElement>) => {
    onChange && onChange({ value: event.target.value });
  };

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  return (
    <select id={id} className="form-select" onChange={onPickerChange} defaultValue={value}>
      {data.map((record, index) => {
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
