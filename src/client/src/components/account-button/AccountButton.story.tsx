import type { Meta, StoryObj } from '@storybook/react';

import { AccountButton } from './AccountButton';

const meta: Meta<typeof AccountButton> = {
  component: AccountButton,
  title: 'Components/AccountButton',
  tags: ['autodocs'],
};

type Story = StoryObj<typeof AccountButton>;

export const Default: Story = {
  render: () => <AccountButton />,
};

export default meta;
