import React, { useState, useEffect } from "react";

function Picker({ id, url, mapper, onChange, onFetch }) {
  const [data, setData] = useState([]);
  const [selectedValue, setSelectedValue] = useState("");

  const fetchData = async () => {
    try {
      let response = await fetch(url);
      let data = await response.json();
      setData(data);
      onFetch && onFetch(data)
    } catch (error) {
      console.error("Error fetching data:", error);
    }
  };

  const onPickerChange = (event) => {
    setSelectedValue(event.target.value);
    onChange && onChange({ value: event.target.value });
  };

  useEffect(() => {
    fetchData();
  }, []);

  return (
    <select id={id} className="form-select" onChange={onPickerChange} value={selectedValue}>
      {data.map((record) => {
        const id = typeof mapper.id === "function" ? mapper.id(record) : record[mapper.id];
        const label = typeof mapper.label === "function" ? mapper.label(record) : record[mapper.label];

        return ((
          <option key={id} value={id}>
            {label}
          </option>
        ));
      })}
    </select>
  );
}

export default Picker;