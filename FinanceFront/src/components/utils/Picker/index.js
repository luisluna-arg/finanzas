import React, { useState, useEffect, useCallback } from "react";

function Picker({ id, value, url, mapper, onChange, onFetch }) {
  const [data, setData] = useState([]);

  const getRecordId = record => {
    let recordIdField = "id";

    if (mapper) {
      if (typeof mapper === "function") recordIdField = mapper(record);

      if (mapper.id) {
        if (typeof mapper.id === "function") recordIdField = mapper.id(record);

        recordIdField = mapper.id;
      }
    }

    return record[recordIdField];
  }

  const getRecordLabel = record =>
    typeof mapper.label === "function" ? mapper.label(record) : record[mapper.label];

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
        const label = getRecordLabel(record);
        const key = `${id}-${recordId}`;

        return ((
          <option key={key} value={recordId}>
            {label}
          </option>
        ));
      })}
    </select>
  );
}

export default Picker;