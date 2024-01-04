import PropTypes from "prop-types";
import React, { useState } from "react";
import { InputControl, InputControlTypes } from '../InputControl';

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

    if (typeof fullProps.value === "undefined") fullProps.value = "";

    return fullProps;
};

const Input = (props) => {
    let { settings, value } = setPropsDefaults(props);

    const [inputValue, setFormValue] = useState(value);

    const handleValueChange = (event) => {
        setFormValue(event.target.value);
    };

    return (
        <div className={[settings.visible ? "" : "d-none"]}>
            <InputControl
                value={inputValue}
                handleValueChange={handleValueChange}
                settings={settings}
            />
        </div>
    );
};

Input.propTypes = {
    value: PropTypes.any,
    settings: PropTypes.any,
};

export default Input;
