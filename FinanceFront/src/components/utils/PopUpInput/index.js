import React, { useState } from 'react'
import PropTypes from 'prop-types'
import Form from 'react-bootstrap/Form'
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { DesktopDateTimePicker } from '@mui/x-date-pickers/DesktopDateTimePicker';
import dateFormat from '../../../utils/dates';
import dayjs from 'dayjs';

const DEFAULTS = {
    value: '',
    settings: {
        id: 'DefaultInput',
        description: 'Default input',
        label: 'Default Input',
        type: 'text',
        placeholder: 'A default input sample',
    },
}

const setPropsDefaults = (originalProps) => {
    let fullProps = Object.assign({}, DEFAULTS, originalProps)
    fullProps.settings = Object.assign(
        {},
        DEFAULTS.settings,
        originalProps.settings,
    )

    if (typeof fullProps.value == 'undefined') fullProps.value = ''

    return fullProps
}

const DateTimeInputControl = (props) => {
    return <div className='mb-2 text-light'>
        <LocalizationProvider dateAdapter={AdapterDayjs}>
            <DesktopDateTimePicker
                defaultValue={dayjs(dateFormat.toRequest())}
                format='DD/MM/YYYY hh:mm A'
                slotProps={{
                    textField: {
                        //required: true,
                        id: props.settings.id ?? ""
                    }
                }}
            />
        </LocalizationProvider>
    </div>;
};

const DecimalInputControl = (props) => {
    return <div className='mb-2 text-light'>
        <input
            id={props.settings.id ?? ""}
            className="mb-2 form-control"
            type="number"
            step="0.01"
        />
    </div>;
}

const InputControl = (props) => {
    switch (props.settings.type) {
        case "DateInput":
            return DateTimeInputControl(props);
        case "DecimalInput":
            return DecimalInputControl(props);
        default: {
            return <Form.Control
                className={"mb-2"}
                id={props.settings.id}
                type={props.settings.type}
                placeholder={props.settings.placeholder}
                value={props.value}
                onChange={props.handleValueChange}
                required
            />;
        }
    }
};

const CRUDPopUpInput = (props) => {
    let { settings, value } = setPropsDefaults(props)

    const [inputValue, setFormValue] = useState(value)

    const handleValueChange = (event) => {
        setFormValue(event.target.value)
    }

    return (
        <div>
            <Form.Group
                id={settings.id + '-group'}
                label-for={settings.id}
                description={settings.description}
            >
                <Form.Label className={["text-light"]}>{settings.label}</Form.Label>

                <InputControl value={inputValue} handleValueChange={handleValueChange} settings={settings} />
            </Form.Group>
        </div>
    )
};

CRUDPopUpInput.propTypes = {
    value: PropTypes.any,
    settings: PropTypes.any,
}

export default CRUDPopUpInput
