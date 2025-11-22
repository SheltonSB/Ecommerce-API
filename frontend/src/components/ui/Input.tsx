import { InputHTMLAttributes } from 'react';
import { cn } from '@/lib/utils';

type Props = InputHTMLAttributes<HTMLInputElement>;

export function Input({ className, ...props }: Props) {
  return (
    <input
      className={cn(
        'w-full rounded-md border border-gray-200 bg-white px-3 py-2 text-sm text-gray-900 placeholder:text-gray-400 shadow-sm',
        'focus:border-indigo-500 focus:outline-none focus:ring-2 focus:ring-indigo-500',
        className
      )}
      {...props}
    />
  );
}
