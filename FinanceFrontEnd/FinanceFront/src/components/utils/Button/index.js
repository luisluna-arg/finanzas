import React from "react";

function Button({ text, type, disabled, variant, className, onClickAction }) {
  return (
    <button
      type={type ?? "button"}
      className={
        "btn btn-" + (variant ?? "primary") + (className ? " " + className : "")
      }
      disabled={disabled}
      onClick={onClickAction}
    >
      {text}
    </button>
  );
}

export default Button;
