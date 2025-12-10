/* eslint-disable react/prop-types */
import React from 'react';
import { Button } from '@/components/ui/shadcn/button';

type ButtonProps = {
  onClick?: () => void;
  disabled?: boolean;
  style?: React.CSSProperties;
  className?: string | string[];
  children?: React.ReactNode;
  type?: 'button' | 'submit' | 'reset';
};

const CustomButton: React.FC<ButtonProps> = ({
  onClick,
  disabled,
  style,
  className,
  children,
  type,
}) => {
  return (
    <Button
      onClick={onClick}
      disabled={disabled}
      style={style}
      type={type}
      className={[
        'font-bold',
        'py-2',
        'px-4',
        'rounded',
        ...(typeof className === 'string' ? [className] : (className ?? [])),
      ].join(' ')}
    >
      {children}
    </Button>
  );
};

export default CustomButton;
