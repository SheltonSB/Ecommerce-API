import { PropsWithChildren } from 'react';
import { cn } from '@/lib/utils';

type Props = PropsWithChildren<{ className?: string }>;

export function Card({ children, className }: Props) {
  return <div className={cn('glass-panel rounded-2xl p-6', className)}>{children}</div>;
}
