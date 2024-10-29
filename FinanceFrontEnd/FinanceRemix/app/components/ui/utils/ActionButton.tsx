import React from "react";
import { Button as BootstrapButton } from 'react-bootstrap';
import { OUTLINE_VARIANTS, BUTTON_OUTLINE_COLOR_VARIANTS } from "./Bootstrap/ColorVariants";

const ActionButton: React.FC<{
  text: string;
  action: () => void;
  disabled?: boolean;
  dataId?: string;
  variant?: BUTTON_OUTLINE_COLOR_VARIANTS;
  width?: string;
  height?: string;
  classes?: string[];
}> = ({ text, action, disabled, dataId, variant = OUTLINE_VARIANTS.PRIMARY, width, height, classes }) => {
  const buttonStyle: React.CSSProperties = {
    width: width || 'auto',
    height: height || 'auto',
  };

  return (
    <BootstrapButton
      onClick={action}
      disabled={disabled}
      variant={variant}
      style={buttonStyle}
      data-id={dataId}
      className={(classes ?? []).join(" ")}
    >
      {text}
    </BootstrapButton>
  );
}

export default ActionButton;
