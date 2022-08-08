import React from 'react';
import PropTypes from 'prop-types';

import Form from 'react-bootstrap/Form';

const CRUDPopUpInput = (props) => {
  return (
    <div>
      <Form.Group id={props.settings.id + '-group'} label-for={props.settings.id}
        description={props.settings.description}>
        <Form.Label>{props.settings.label}</Form.Label>
        <Form.Input id={props.settings.id} type={props.settings.type} placeholder={props.settings.placeholder}
          value={props.value} required />
      </Form.Group>
    </div>
  )
};

CRUDPopUpInput.propTypes = {
  value: PropTypes.any,
  settings: PropTypes.any
};

CRUDPopUpInput.defaultProps = {
  value: null,
  settings: {
    id: 'DefaultInput',
    description: 'Default input',
    label: 'Default Input',
    type: 'text',
    placeholder: 'A default input sample'
  }
};

export default CRUDPopUpInput;
