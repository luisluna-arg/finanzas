import * as React from 'react';
import { CheckIcon } from 'lucide-react';

import { cn } from '@/lib/utils';

interface CheckboxProps extends Omit<React.ComponentProps<'input'>, 'type' | 'onChange'> {
  checked?: boolean;
  onCheckedChange?: (checked: boolean) => void;
  onChange?: (event: React.ChangeEvent<HTMLInputElement>) => void;
}

function Checkbox({ 
  className, 
  checked, 
  onCheckedChange, 
  onChange,
  disabled,
  ...props 
}: CheckboxProps) {
  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (onChange) {
      onChange(event);
    }
    if (onCheckedChange) {
      onCheckedChange(event.target.checked);
    }
  };

  return (
    <div className="relative inline-flex items-center align-middle [&_input]:!h-auto [&_input]:!w-auto">
      <input
        type="checkbox"
        checked={checked}
        onChange={handleChange}
        disabled={disabled}
        className="sr-only peer !h-auto !w-auto"
        {...props}
      />
      <div
        className={cn(
          'size-4 shrink-0 rounded-[4px] border transition-all cursor-pointer outline-none',
          'border-gray-200 bg-white shadow-xs dark:border-gray-800 dark:bg-gray-200/30',
          'peer-checked:bg-gray-900 peer-checked:text-white peer-checked:border-gray-900',
          'peer-focus-visible:ring-[3px] peer-focus-visible:ring-gray-950/50 peer-focus-visible:border-gray-950',
          'dark:peer-checked:bg-gray-50 dark:peer-checked:text-gray-900 dark:peer-checked:border-gray-50',
          'dark:peer-focus-visible:ring-gray-300/50 dark:peer-focus-visible:border-gray-300',
          'peer-disabled:cursor-not-allowed peer-disabled:opacity-50',
          'flex items-center justify-center',
          className
        )}
      >
        {checked && <CheckIcon className="size-3.5 stroke-current" />}
      </div>
    </div>
  );
}

export { Checkbox };
