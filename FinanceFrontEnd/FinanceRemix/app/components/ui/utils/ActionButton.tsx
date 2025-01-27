import React from "react";
import Button from "@/components/ui/utils/Button";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faAdd,
  faBan,
  faPencilAlt,
  faTrashAlt,
} from "@fortawesome/free-solid-svg-icons";
import {
  OUTLINE_VARIANT,
  BUTTON_OUTLINE_COLOR_VARIANT,
} from "./Bootstrap/ColorVariant";

export enum ButtonType {
  None,
  Edit,
  Delete,
  Add,
}

const ButtonIcon: React.FC<{
  type?: ButtonType;
}> = ({ type }) => {
  let icon = null;
  switch (type) {
    case ButtonType.Add:
      icon = faAdd;
      break;
    case ButtonType.Edit:
      icon = faPencilAlt;
      break;
    case ButtonType.Delete:
      icon = faTrashAlt;
      break;
    default:
      icon = faBan;
      break;
  }

  return <FontAwesomeIcon icon={icon} size={"xs"} className={"me-2"} />;
};

const ActionButton: React.FC<{
  text?: string;
  type?: ButtonType;
  action: () => void;
  disabled?: boolean;
  dataId?: string;
  variant?: BUTTON_OUTLINE_COLOR_VARIANT;
  width?: string;
  height?: string;
  classes?: string[];
}> = ({
  text,
  type,
  action,
  disabled,
  dataId,
  variant = OUTLINE_VARIANT.PRIMARY,
  width,
  height,
  classes,
}) => {
  const buttonStyle: React.CSSProperties = {
    width: width || "auto",
    height: height || "auto",
    flexFlow: "row",
  };

  return (
    <Button
      onClick={action}
      disabled={disabled}
      variant={variant}
      style={buttonStyle}
      dataId={dataId}
      className={(classes ?? []).concat(["centered"]).join(" ")}
    >
      <ButtonIcon type={type} />
      {text}
    </Button>
  );
};

export default ActionButton;
