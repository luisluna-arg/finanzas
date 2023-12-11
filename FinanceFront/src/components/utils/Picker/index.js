import React, { useState, useEffect } from "react";

function Picker({ id, url, mapper, onChange, onFetch }) {
  const [data, setData] = useState([]);
  const [selectedValue, setSelectedValue] = useState("");

  const fetchData = async () => {
    try {
      let response = await fetch(url);
      let data = await response.json();
      setData(data);
      onFetch(data)
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
      {data.map((record) => (
        <option key={record[mapper.id]} value={record[mapper.id]}>
          {record[mapper.label]}
        </option>
      ))}
    </select>
  );
}

export default Picker;