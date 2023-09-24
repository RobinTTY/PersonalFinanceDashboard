import { IconUserPlus, IconDiscount2, IconReceipt2, IconCoin } from '@tabler/icons-react';

export const icons = {
  user: IconUserPlus,
  discount: IconDiscount2,
  receipt: IconReceipt2,
  coin: IconCoin,
};

export interface StatCardProps {
  title: string;
  icon: keyof typeof icons;
  value: string;
  diff: number;
}
