/* eslint-disable react/prop-types */
import React from "react";

type ButtonProps = {
    onClick?: () => void;
    disabled?: boolean;
    style?: React.CSSProperties;
    className?: string | string[];
    children?: React.ReactNode;
};

const Button: React.FC<ButtonProps> = ({
    onClick,
    disabled,
    style,
    className,
    children,
}) => {
    return (
        <button
            onClick={onClick}
            disabled={disabled}
            style={style}
            className={[
                "font-bold",
                "py-2",
                "px-4",
                "rounded",
                ...(typeof className === "string"
                    ? [className]
                    : className ?? []),
            ].join(" ")}
        >
            {children}
        </button>
    );
};

export default Button;
