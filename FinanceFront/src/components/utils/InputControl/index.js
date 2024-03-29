import React from "react";
import Picker from '../Picker';

export const InputControlTypes = {
    Boolean: "BooleanInput",
    DateTime: "DateTimeInput",
    Decimal: "DecimalInput",
    Dropdown: "DropdownInput",
    Integer: "IntegerInput",
    Text: "TextInput",
};

export const InputControl = (props) => {

    const classesToString = (classes) => classes.reduce((curr, result) => `${curr} ${result}`);

    const nullablePropertyResolver = (name, property) => {
        if (property !== undefined && property !== null) {
            let result = {};
            result[name] = property;
            return result;
        }
    };

    // const DateTimeInputControl = (props) => {
    //     return (<div className='mb-2 text-light'>
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
    //     </div>);
    //     /*
    //     timeFormat: "HH:mm",
    //     timeIntervals: 15,
    //     dateFormat: "DD/MM/YYYY HH:mm",
    //     placeholder
    //     */
    // };

    const IntegerInputControl = (props) => {
        const input = (
            <input
                id={props.settings.id ?? ""}
                type="number"
                className={classesToString([
                    "form-control",
                    props.settings.visible ? "visible" : "invisible",
                ])}
                step="1"
                {...(nullablePropertyResolver("min", props.settings.min))}
                pattern="\d+"
                title="Ingresar un número entero válido"
                style={props.settings.style ?? {}}
                defaultValue={props?.value}
            />
        );

        return input;
    };

    const DecimalInputControl = (props) => {
        console.log(`props?.value: ${props?.value}`);
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
                defaultValue={props?.value}
            />
        );
    };

    const BooleanInputControl = (props) => {
        return (
            <div className="mb-2 text-light">
                <input
                    id={props.settings.id ?? ""}
                    className={classesToString([
                        "form-check-input",
                        props.settings.visible ? "visible" : "invisible",
                    ])}
                    type="checkbox"
                    value={props?.value}
                    style={props.settings.style ?? {}}
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
                style={props.settings.style ?? {}}
            />
        );
    };

    switch (props.settings.type) {
        // case InputControlTypes.DateTime:
        //     return DateTimeInputControl(props);
        case InputControlTypes.Integer:
            return IntegerInputControl(props);
        case InputControlTypes.Decimal:
            return DecimalInputControl(props);
        case InputControlTypes.Boolean:
            return BooleanInputControl(props);
        case InputControlTypes.Dropdown:
            return DropdownInput(props);
        default: {
            return (
                <input
                    id={props.settings.id}
                    placeholder={props.settings.placeholder}
                    type="text"
                    style={props.settings.style ?? {}}
                    className={[
                        props.settings.className ?? '',
                        "form-control",
                        props.settings.visible ? "visible" : "invisible",
                    ].reduce((current, value) => `${current} ${value}`)}
                    value={props.value}
                    onChange={props.handleValueChange}
                    required />);
            // #############################
            // return (
            //     <Form.Control
            //         className={[
            //             "mb-2 ",
            //             props.settings.visible ? "visible" : "invisible",
            //         ]}
            //         id={props.settings.id}
            //         type={props.settings.type}
            //         placeholder={props.settings.placeholder}
            //         value={props.value}
            //         onChange={props.handleValueChange}
            //         required
            //     />
            // );
        }
    }
};
