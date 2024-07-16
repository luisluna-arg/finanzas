import React, { useState, ChangeEvent } from "react";
import { InputControl, InputControlTypes, InputControlSettings } from '../InputControl';

const DEFAULTS = {
  value: "",
  settings: {
    id: "DefaultInput",
    description: "Default input",
    label: "Default Input",
    type: InputControlTypes.Text, // Ensure this matches your InputControlTypes enum
    placeholder: "A default input sample",
    visible: true,
  },
};

interface InputProps {
  value?: any;
  settings: InputControlSettings;
}

const setPropsDefaults = (originalProps: InputProps): InputProps => {
  let fullProps = { ...DEFAULTS, ...originalProps };
  fullProps.settings = { ...DEFAULTS.settings, ...originalProps.settings };

  if (typeof fullProps.value === "undefined") fullProps.value = "";

  return fullProps;
};

const Input: React.FC<InputProps> = (props) => {
  let { settings, value } = setPropsDefaults(props);

  const [inputValue, setFormValue] = useState(value);

  const handleValueChange = (event: ChangeEvent<HTMLInputElement>) => {
    setFormValue(event.target.value);
  };

  return (
    <div className={settings!.visible ? "" : "d-none"}>
      <InputControl
        value={inputValue}
        handleValueChange={handleValueChange}
        settings={settings}
      />
    </div>
  );
};

export default Input;
