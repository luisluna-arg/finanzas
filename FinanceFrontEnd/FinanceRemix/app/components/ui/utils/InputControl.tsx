import React from "react";
import { InputType } from "@/components/ui/utils/InputType";
import Picker, { MapperType } from "@/components/ui/utils/Picker";
import { Input } from "@/components/ui/shadcn/input";

export interface Settings {
  id?: string;
  type: InputType;
  description?: string;
  label?: string;
  placeholder?: string;
  className?: string;
  visible?: boolean;
  style?: React.CSSProperties;
  min?: number;
  endpoint?: string;
  mapper?: MapperType;
}

interface InputControlProps {
  value?: string | number | boolean;
  settings: Settings;
  handleValueChange?: (event: React.ChangeEvent<HTMLInputElement>) => void;
}

const classesToString = (classes: string[]): string => classes.join(" ");

const nullablePropertyResolver = (name: string, property: any) => {
  if (property !== undefined && property !== null) {
    return { [name]: property };
  }
  return {};
};

// Input Components

const IntegerInputControl: React.FC<InputControlProps> = ({
  settings,
  value,
}) => (
  <Input
    id={settings.id}
    type="number"
    className={classesToString([
      "form-control",
      settings.visible ? "visible" : "invisible",
    ])}
    step="1"
    {...nullablePropertyResolver("min", settings.min)}
    pattern="\d+"
    title="Ingresar un número entero válido"
    style={settings.style}
    defaultValue={value as string | number}
  />
);

const DecimalInputControl: React.FC<InputControlProps> = ({
  settings,
  value,
}) => (
  <Input
    id={settings.id}
    type="number"
    className={classesToString([
      "form-control",
      settings.visible ? "visible" : "invisible",
    ])}
    {...nullablePropertyResolver("min", settings.min)}
    step="0.01"
    pattern="\d+(\.\d{2})?"
    title="Ingresar un número decimal válido"
    style={settings.style}
    defaultValue={value as string | number}
  />
);

const BooleanInputControl: React.FC<InputControlProps> = ({
  settings,
  value,
}) => (
  <div className="mb-2 text-white">
    <Input
      id={settings.id}
      className={classesToString([
        "form-check-input",
        settings.visible ? "visible" : "invisible",
      ])}
      type="checkbox"
      defaultChecked={Boolean(value)}
      style={settings.style}
    />
  </div>
);

const DropdownInput: React.FC<InputControlProps> = ({ settings }) => (
  <Picker
    id={settings.id ?? ""}
    url={settings.endpoint || ""}
    mapper={settings.mapper}
    // style={settings.style}
  />
);

// Main InputControl Component

const InputControl: React.FC<InputControlProps> = (props) => {
  const { settings, value, handleValueChange } = props;

  switch (settings.type) {
    case InputType.Decimal:
      return <IntegerInputControl {...props} />;
    case InputType.Decimal:
      return <DecimalInputControl {...props} />;
    case InputType.Boolean:
      return <BooleanInputControl {...props} />;
    case InputType.Dropdown:
      return <DropdownInput {...props} />;
    default:
      return (
        <Input
          id={settings.id}
          placeholder={settings.placeholder}
          type="text"
          style={settings.style}
          className={classesToString([
            settings.className ?? "",
            "form-control",
            settings.visible ? "visible" : "invisible",
          ])}
          value={value as string}
          onChange={handleValueChange}
          required
        />
      );
  }
};

export default InputControl;
