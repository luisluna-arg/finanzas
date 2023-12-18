import PropTypes from "prop-types";
import React, { useState } from "react";
import Form from "react-bootstrap/Form";
import Picker from '../Picker';

const DEFAULTS = {
  value: "",
  settings: {
    id: "DefaultInput",
    description: "Default input",
    label: "Default Input",
    type: "text",
    placeholder: "A default input sample",
    visible: true
  },
};

const setPropsDefaults = (originalProps) => {
  let fullProps = Object.assign({}, DEFAULTS, originalProps);
  fullProps.settings = Object.assign(
    {},
    DEFAULTS.settings,
    originalProps.settings
  );

  if (typeof fullProps.value == "undefined") fullProps.value = "";

  return fullProps;
};

// const DateTimeInputControl = (props) => {
//     return <div className='mb-2 text-light'>
//         <LocalizationProvider dateAdapter={AdapterDayjs}>
//             <DesktopDateTimePicker
//                 defaultValue={dayjs(dateFormat.toRequest())}
//                 format='DD/MM/YYYY hh:mm A'
//                 slotProps={{
//                     textField: {
//                         //required: true,
//                         id: props.settings.id ?? ""
//                     }
//                 }}
//             />
//         </LocalizationProvider>
//     </div>;
// };

const DecimalInputControl = (props) => {
  return (
    <div className="mb-2 text-light">
      <input
        id={props.settings.id ?? ""}
        className={[
          "mb-2",
          "form-control",
          props.settings.visible ? "visible" : "invisible",
        ]}
        type="number"
        step="0.01"
      />
    </div>
  );
};

const BooleanInputControl = (props) => {
  return (
    <div className="mb-2 text-light">
      <input
        id={props.settings.id ?? ""}
        className={[
          "form-check-input",
          "mt-0",
          props.settings.visible ? "visible" : "invisible",
        ]}
        type="checkbox"
        value=""
      />
    </div>
  );
};

const DropdownInput = (props) => {
  return (
    <Picker
      id={props.settings.id}
      url={props.settings.endpoint}
      mapper={props.settings.mapper}
      />
  );
};

const InputControl = (props) => {
  switch (props.settings.type) {
    // case "DateInput":
    //   return DateTimeInputControl(props);
    case "DecimalInput":
      return DecimalInputControl(props);
    case "BooleanInput":
      return BooleanInputControl(props);
    case "DropdownInput":
      return DropdownInput(props);
    default: {
      return (
        <Form.Control
          className={[
            "mb-2 ",
            props.settings.visible ? "visible" : "invisible",
          ]}
          id={props.settings.id}
          type={props.settings.type}
          placeholder={props.settings.placeholder}
          value={props.value}
          onChange={props.handleValueChange}
          required
        />
      );
    }
  }
};

const CRUDPopUpInput = (props) => {
  let { settings, value } = setPropsDefaults(props);

  const [inputValue, setFormValue] = useState(value);

  const handleValueChange = (event) => {
    setFormValue(event.target.value);
  };

  return (
    <div className={[settings.visible ? "" : "d-none"]}>
      <Form.Group
        id={settings.id + "-group"}
        label-for={settings.id}
        description={settings.description}
      >
        {settings.visible && <Form.Label>{settings.label}</Form.Label>}

        <InputControl
          value={inputValue}
          handleValueChange={handleValueChange}
          settings={settings}
        />
      </Form.Group>
    </div>
  );
};

CRUDPopUpInput.propTypes = {
  value: PropTypes.any,
  settings: PropTypes.any,
};

export default CRUDPopUpInput;
