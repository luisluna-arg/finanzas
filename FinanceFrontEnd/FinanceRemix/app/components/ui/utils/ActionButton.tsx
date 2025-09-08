import { Button } from "@/components/ui/shadcn/button";
import { cn } from "@/lib/utils";
import { CirclePlus, Eraser, SendHorizonal, Trash2Icon, SquarePen } from "lucide-react";
import SafeLogger from "@/utils/SafeLogger";

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

export type VariantType = string | null | undefined;

export enum ButtonType {
  None = "none",
  Submit = "submit",
}

export interface ActionButtonProps {
  type?: ActionButtonType | ButtonType;
  text?: string;
  className?: string | Array<string>;
  variant?: VariantType;
  disabled?: boolean;
  onClick?: (e?: any) => Promise<void> | (() => void) | void;
  // older components pass `action` prop name
  action?: (() => void) | ((e?: any) => void) | Promise<void>;
  // optional explicit button type enum used by some callers
  buttonType?: ButtonType;
}

const ActionButton = ({
  type,
  text,
  className,
  variant,
  disabled,
  onClick,
  action,
  buttonType,
}: ActionButtonProps) => {
  let icon: JSX.Element | null = null;
  let classes: Array<string> = [];

  if (type && typeof type === "string" && (type in icons)) {
    icon = icons[type as ActionButtonType];
    classes = buttonClasses[type as ActionButtonType] ?? [];
  }

  return (
    <Button
  variant={(variant as any) ?? "outline"}
  size={text ? undefined : "icon"}
      className={cn(classes, className)}
      onClick={(e: any) => {
        const fn = onClick ?? action;
        if (!fn) return;
        try {
          const res = (fn as any)(e);
          if (res && typeof res.then === "function") {
            res.catch((err: any) => SafeLogger.error(err));
          }
        } catch (err) {
          SafeLogger.error(err);
        }
      }}
  type={type === "submit" || buttonType === ButtonType.Submit ? ("submit" as any) : undefined}
      disabled={disabled ?? false}
    >
      {icon} {text ?? ""}
    </Button>
  );
};

export default ActionButton;
