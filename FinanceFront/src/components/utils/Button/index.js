import React from "react";

function Button({ text, type, disabled, variant, onClickAction }) {
  return (
    <button
      type={type ?? "button"}
      className={"btn btn-" + (variant ?? "primary")}
      disabled={disabled}
      onClick={onClickAction}
    >
      {text}
    </button>
  );
}

export default Button;
