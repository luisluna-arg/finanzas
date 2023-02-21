import React, { useState } from 'react'
import PropTypes from 'prop-types'
import Form from 'react-bootstrap/Form'

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

const CRUDPopUpInput = (props) => {
  let { settings, value } = setPropsDefaults(props)

  const [formValue, setFormValue] = useState(value)

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
        <Form.Label>{settings.label}</Form.Label>
        <Form.Control
          id={settings.id}
          type={settings.type}
          placeholder={settings.placeholder}
          value={formValue}
          onChange={handleValueChange}
          required
        />
      </Form.Group>
    </div>
  )
}

CRUDPopUpInput.propTypes = {
  value: PropTypes.any,
  settings: PropTypes.any,
}

export default CRUDPopUpInput
