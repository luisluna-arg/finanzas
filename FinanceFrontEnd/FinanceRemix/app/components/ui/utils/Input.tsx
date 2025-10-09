import React, { useState } from "react";
import { InputType } from "@/components/ui/utils/InputType";
import InputControl, {
    Settings as InputControlSettings,
} from "@/components/ui/utils/InputControl";

interface InputProps {
    value?: string;
    settings?: InputControlSettings;
}

const DEFAULTS: Required<InputProps> = {
    value: "",
    settings: {
        id: "DefaultInput",
        description: "Default input",
        label: "Default Input",
        type: InputType.None,
        placeholder: "A default input sample",
        visible: true,
    },
};

const setPropsDefaults = (originalProps: InputProps): Required<InputProps> => {
    const fullProps = { ...DEFAULTS, ...originalProps };
    fullProps.settings = { ...DEFAULTS.settings, ...originalProps.settings };

    if (typeof fullProps.value === "undefined") fullProps.value = "";

    return fullProps;
};

const Input: React.FC<InputProps> = (props) => {
    const { settings, value } = setPropsDefaults(props);

    const [inputValue, setInputValue] = useState<string>(value);

    const handleValueChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setInputValue(event.target.value);
    };

    return (
        <div className={settings.visible ? "" : "d-none"}>
            <InputControl
                value={inputValue}
                handleValueChange={handleValueChange}
                settings={settings}
            />
        </div>
    );
};

export default Input;
