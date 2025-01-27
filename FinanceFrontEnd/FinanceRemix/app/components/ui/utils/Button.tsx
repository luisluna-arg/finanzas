type ButtonProps = {
  onClick: () => void;
  disabled: boolean | undefined;
  variant: string;
  style: any;
  dataId: string | undefined;
  className: string | string[];
  children: any;
};

const Button: React.FC<ButtonProps> = ({
  onClick,
  disabled,
  variant,
  style,
  dataId,
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
        ...(typeof className === "string" ? [className] : className ?? []),
      ].join(" ")}
    >
      {children}
    </button>
  );
};

export default Button;
