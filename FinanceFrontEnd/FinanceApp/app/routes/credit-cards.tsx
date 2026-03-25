import { cn } from '@/lib/utils';
import { Outlet } from 'react-router';

export default function CreditCardsLayout() {
  return (
    <div className={cn('py-10', 'px-40')}>
      <Outlet />
    </div>
  );
}
