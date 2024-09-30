import React from "react";
import { Button as BootstrapButton } from 'react-bootstrap';
import { BUTTON_COLOR_VARIANTS, VARIANTS } from "./Bootstrap/ColorVariants";

interface ButtonProps {
  text: React.ReactNode;
  type?: "button" | "submit" | "reset";
  disabled?: boolean;
  variant?: "primary" | "secondary" | "success" | "danger" | "warning" | "info" | "light" | "dark";
  className?: string;
  onClickAction?: (event: React.MouseEvent<HTMLButtonElement>) => void;
}

const ActionButton: React.FC<{
  text: string;
  action: () => void;
  disabled?: boolean;
  dataId?: string;
  variant?: BUTTON_COLOR_VARIANTS;
  width?: string;
  height?: string;
  classes?: string[];
}> = ({ text, action, disabled, dataId, variant = VARIANTS.PRIMARY, width, height, classes }) => {
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
