import { ButtonHTMLAttributes, PropsWithChildren } from 'react';
import { cn } from '../../utils/cn';

type ButtonProps = ButtonHTMLAttributes<HTMLButtonElement> & {
  variant?: 'primary' | 'secondary';
};

export function Button({ children, className, variant = 'primary', ...props }: PropsWithChildren<ButtonProps>) {
  const base =
    'inline-flex items-center justify-center rounded-md px-4 py-2 text-sm font-medium transition-all focus:outline-none focus:ring-2 focus:ring-offset-2';
  const styles =
    variant === 'primary'
      ? 'bg-indigo-600 text-white hover:bg-indigo-700 focus:ring-indigo-500 focus:ring-offset-gray-50'
      : 'bg-white text-gray-900 border border-gray-200 hover:border-gray-300 focus:ring-indigo-500';

  return (
    <button className={cn(base, styles, className)} {...props}>
      {children}
    </button>
  );
}
