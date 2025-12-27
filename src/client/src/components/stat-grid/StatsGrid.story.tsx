import type { Meta, StoryObj } from '@storybook/react-vite';
import { StatsGrid } from './StatsGrid';
import { StatCardProps } from '../stat-card/StatCardProps';

const meta: Meta<typeof StatsGrid> = {
  component: StatsGrid,
  title: 'Components/StatsGrid',
  tags: ['autodocs'],
};

type Story = StoryObj<typeof StatsGrid>;

const gridprops: StatCardProps[] = [
  { title: 'Net Worth', icon: 'coin', value: '$13,456', diff: 34 },
  { title: 'Profit', icon: 'receipt', value: '$145.56', diff: -13 },
  { title: 'Transactions', icon: 'discount', value: '23', diff: 8 },
  {
    title: 'Followers',
    icon: 'user',
    value: '1835',
    diff: 3,
  },
];

export const Default: Story = {
  render: () => <StatsGrid data={gridprops} />,
};

export default meta;
