import { PropsWithChildren } from 'react';
import { cn } from '@/lib/utils';

type Props = PropsWithChildren<{ className?: string }>;

export function Card({ children, className }: Props) {
  return <div className={cn('rounded-xl border border-gray-100 bg-white p-6 shadow-sm', className)}>{children}</div>;
}
