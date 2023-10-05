import type { Meta, StoryObj } from '@storybook/react';
import { StatCard } from './StatCard';

const meta: Meta<typeof StatCard> = {
  component: StatCard,
  title: 'Components/StatCard',
  tags: ['autodocs'],
};

type Story = StoryObj<typeof StatCard>;

// TODO: icon should be inserted as JSX.Element
export const Default: Story = {
  render: () => <StatCard title="Net Worth" icon="coin" value="$13,456" diff={34} />,
};

export default meta;
