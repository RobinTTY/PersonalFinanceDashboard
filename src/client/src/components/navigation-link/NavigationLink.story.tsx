import type { Meta, StoryObj } from '@storybook/react';
import { IconHome } from '@tabler/icons-react';
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
    <NavigationLink icon={<IconHome size="1.25rem" />} color="blue" label="Dashboard" />
  ),
};

export default meta;
