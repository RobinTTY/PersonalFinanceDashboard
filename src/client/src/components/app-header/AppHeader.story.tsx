import type { Meta, StoryObj } from '@storybook/react';

import { AppHeader } from './AppHeader';

const meta: Meta<typeof AppHeader> = {
  component: AppHeader,
  title: 'Components/AppHeader',
  tags: ['autodocs'],
};

type Story = StoryObj<typeof AppHeader>;

export const Default: Story = {
  render: () => <AppHeader />,
};

export default meta;
