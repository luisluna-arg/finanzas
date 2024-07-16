import React, { FunctionComponent, MouseEventHandler } from "react";

interface ButtonProps {
  text: string;
  type?: "button" | "submit" | "reset";
  disabled?: boolean;
  variant?: string;
  className?: string;
  onClickAction?: MouseEventHandler<HTMLButtonElement>;
}

const Button: FunctionComponent<ButtonProps> = ({
  text,
  type = "button",
  disabled,
  variant = "primary",
  className,
  onClickAction,
}) => {
  return (
    <button
      type={type}
      className={`btn btn-${variant}${className ? " " + className : ""}`}
      disabled={disabled}
      onClick={onClickAction}
    >
      {text}
    </button>
  );
};

export default Button;
