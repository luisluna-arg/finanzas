import React, { ChangeEvent, ChangeEventHandler, FunctionComponent } from "react";
import Picker from '../Picker';

export enum InputControlTypes {
    Boolean = "BooleanInput",
    DateTime = "DateTimeInput",
    Decimal = "DecimalInput",
    Dropdown = "DropdownInput",
    Integer = "IntegerInput",
    Text = "TextInput",
}

export interface InputControlSettings {
    id: string;
    visible?: boolean;
    style?: React.CSSProperties;
    min?: number;
    endpoint: string;
    mapper?: any;
    placeholder?: string;
    className?: string;
    type: InputControlTypes;
}

export interface InputControlProps {
    settings: InputControlSettings;
    value?: any;
    handleValueChange?: any;
}

export const InputControl: FunctionComponent<InputControlProps> = (props) => {
    const classesToString = (classes: string[]): string =>
        classes.reduce((curr, result) => `${curr} ${result}`, "");

    const nullablePropertyResolver = (name: string, property: any) => {
        if (property !== undefined && property !== null) {
            let result: { [key: string]: any } = {};
            result[name] = property;
            return result;
        }
        return {};
    };

    const IntegerInputControl: FunctionComponent<InputControlProps> = (props) => {
        return (
            <input
                id={props.settings.id ?? ""}
                type="number"
                className={classesToString([
                    "form-control",
                    props.settings.visible ? "visible" : "invisible",
                ])}
                step="1"
                {...nullablePropertyResolver("min", props.settings.min)}
                pattern="\d+"
                title="Ingresar un número entero válido"
                style={props.settings.style ?? {}}
                defaultValue={props.value}
            />
        );
    };

    const DecimalInputControl: FunctionComponent<InputControlProps> = (props) => {
        return (
            <input
                id={props.settings.id ?? ""}
                type="number"
                className={classesToString([
                    "form-control",
                    props.settings.visible ? "visible" : "invisible",
                ])}
                {...nullablePropertyResolver("min", props.settings.min)}
                step="0.01"
                pattern="\d+(\.\d{2})?"
                title="Ingresar un número decimal válido"
                style={props.settings.style ?? {}}
                defaultValue={props.value}
            />
        );
    };

    const BooleanInputControl: FunctionComponent<InputControlProps> = (props) => {
        return (
            <div className="mb-2 text-light">
                <input
                    id={props.settings.id ?? ""}
                    className={classesToString([
                        "form-check-input",
                        props.settings.visible ? "visible" : "invisible",
                    ])}
                    type="checkbox"
                    checked={props.value}
                    style={props.settings.style ?? {}}
                />
            </div>
        );
    };

    const DropdownInput: FunctionComponent<InputControlProps> = (props) => {
        return (
            <Picker
                id={props.settings.id}
                value={props.value}
                url={props.settings.endpoint}
                mapper={props.settings.mapper}
                onChange={props.handleValueChange}
            />
        );
    };

    switch (props.settings.type) {
        // case InputControlTypes.DateTime:
        //     return DateTimeInputControl(props);
        case InputControlTypes.Integer:
            return <IntegerInputControl {...props} />;
        case InputControlTypes.Decimal:
            return <DecimalInputControl {...props} />;
        case InputControlTypes.Boolean:
            return <BooleanInputControl {...props} />;
        case InputControlTypes.Dropdown:
            return <DropdownInput {...props} />;
        default:
            return (
                <input
                    id={props.settings.id}
                    placeholder={props.settings.placeholder}
                    type="text"
                    style={props.settings.style ?? {}}
                    className={[
                        props.settings.className ?? "",
                        "form-control",
                        props.settings.visible ? "visible" : "invisible",
                    ].reduce((current, value) => `${current} ${value}`)}
                    value={props.value}
                    onChange={props.handleValueChange}
                    required
                />
            );
    }
};
