import React from "react";

interface ButtonProps {
  text: React.ReactNode;
  type?: "button" | "submit" | "reset";
  disabled?: boolean;
  variant?: "primary" | "secondary" | "success" | "danger" | "warning" | "info" | "light" | "dark";
  className?: string;
  onClickAction?: (event: React.MouseEvent<HTMLButtonElement>) => void;
}

const Button: React.FC<ButtonProps> = ({
  text,
  type = "button",
  disabled = false,
  variant = "primary",
  className = "",
  onClickAction
}) => {
  return (
    <button
      type={type}
      className={`btn btn-${variant} ${className}`}
      disabled={disabled}
      onClick={onClickAction}
    >
      {text}
    </button>
  );
};

export default Button;
