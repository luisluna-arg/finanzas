import { Button } from "@/components/ui/shadcn/button";
import { cn } from "@/lib/utils";
import { CirclePlus, Eraser, SendHorizonal, Trash2Icon, SquarePen } from "lucide-react";

export const ACTION_BUTTON_KIND = {
  add: "add",
  delete: "delete",
  submit: "submit",
  clear: "clear",
  edit: "edit"
} as const;

export type ActionButtonType = keyof typeof ACTION_BUTTON_KIND;

const icons: Record<ActionButtonType, JSX.Element> = {
  add: <CirclePlus />,
  delete: <Trash2Icon />,
  submit: <SendHorizonal />,
  clear: <Eraser />,
  edit: <SquarePen />,
};

const buttonClasses: Record<ActionButtonType, Array<string>> = {
  add: ["bg-green-100", "hover:bg-green-400", "hover:text-white"],
  delete: ["bg-red-100", "hover:bg-red-600", "hover:text-white"],
  submit: ["bg-green-100", "hover:bg-green-600", "hover:text-white"],
  clear: ["bg-amber-100", "hover:bg-amber-400", "hover:text-white"],
  edit: ["bg-cyan-100", "hover:bg-cyan-400", "hover:text-white"],
};

export const VARIANTS = {
  link: "link",
  default: "default",
  destructive: "destructive",
  outline: "outline",
  secondary: "secondary",
  ghost: "ghost",
} as const;

export type VariantType = keyof typeof VARIANTS | null | undefined;

export interface ActionButtonProps {
  type: ActionButtonType;
  text?: string;
  className?: string | Array<string>;
  variant?: VariantType;
  disabled?: boolean;
  onClick?: (e?: any) => Promise<void> | (() => void) | void;
}

const ActionButton = ({
  type,
  text,
  className,
  variant,
  disabled,
  onClick,
}: ActionButtonProps) => {
  const icon = icons[type];

  const classes = buttonClasses[type];

  return (
    <Button
      variant={variant ?? "outline"}
      size={text ? undefined : "icon"}
      className={cn(classes, className)}
      onClick={onClick}
      type={type === "submit" ? type : undefined}
      disabled={disabled ?? false}
    >
      {icon} {text ?? ""}
    </Button>
  );
};

export default ActionButton;
