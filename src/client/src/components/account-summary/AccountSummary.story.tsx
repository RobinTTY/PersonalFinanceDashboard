import type { Meta, StoryObj } from '@storybook/react';

import { AccountSummary } from './AccountSummary';

const meta: Meta<typeof AccountSummary> = {
  component: AccountSummary,
  title: 'Components/AccountSummary',
};

type Story = StoryObj<typeof AccountSummary>;

export const Default: Story = {
  render: () => <AccountSummary description="Checkings" balance={1753.54} currency="USD" />,
};

export default meta;
