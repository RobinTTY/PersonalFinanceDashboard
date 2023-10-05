import type { Meta, StoryObj } from '@storybook/react';
import { Shell } from './Shell';
import { BrowserRouter } from 'react-router-dom';

const meta: Meta<typeof Shell> = {
  component: Shell,
  title: 'Components/Shell',
  tags: ['autodocs'],
};

type Story = StoryObj<typeof Shell>;

// TODO: Fix link styling, remove inherit in index.css
export const Default: Story = {
  render: () => (
    <BrowserRouter>
      <Shell />
    </BrowserRouter>
  ),
};

export default meta;
