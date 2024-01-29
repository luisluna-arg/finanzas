import React, { useState, useEffect, useCallback } from "react";

function Picker({ id, value, url, mapper, onChange, onFetch }) {
  const [data, setData] = useState([]);

  const getRecordId = record => {
    return typeof mapper.id === "function" ? mapper.id(record) : record[mapper.id];
  }

  const fetchData = useCallback(async () => {
    try {
      let data = await (await fetch(url)).json();
      setData(data);
      onFetch && onFetch({ data })
    } catch (error) {
      console.error("Error fetching data:", error);
    }
  }, []);

  const onPickerChange = (event) => {
    onChange && onChange({ value: event.target.value });
  };

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  return (
    <select id={id} className="form-select" onChange={onPickerChange} defaultValue={value}>
      {data.map((record, index) => {
        const recordId = getRecordId(record);
        const label = typeof mapper.label === "function" ? mapper.label(record) : record[mapper.label];

        return ((
          <option key={recordId} value={recordId}>
            {label}
          </option>
        ));
      })}
    </select>
  );
}

export default Picker;