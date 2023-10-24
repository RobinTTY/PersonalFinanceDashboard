import type { Meta, StoryObj } from '@storybook/react';
import { IconHome } from '@tabler/icons-react';
import { BrowserRouter } from 'react-router-dom';
import { NavigationLink } from './NavigationLink';

const meta: Meta<typeof NavigationLink> = {
  component: NavigationLink,
  title: 'Components/NavigationLink',
  tags: ['autodocs'],
};

type Story = StoryObj<typeof NavigationLink>;

// TODO: this is basically the same component as AccountButton, should be combined
export const Default: Story = {
  render: () => (
    <BrowserRouter>
      <NavigationLink
        icon={<IconHome size="1.25rem" />}
        color="blue"
        label="Dashboard"
        to="dashboard"
      />
    </BrowserRouter>
  ),
};

export default meta;
