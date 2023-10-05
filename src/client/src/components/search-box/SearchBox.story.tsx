import type { Meta, StoryObj } from '@storybook/react';
import { SearchBox } from './SearchBox';

const meta: Meta<typeof SearchBox> = {
  component: SearchBox,
  title: 'Components/SearchBox',
  tags: ['autodocs'],
};

type Story = StoryObj<typeof SearchBox>;

// TODO: this is basically the same component as AccountButton, should be combined
export const Default: Story = {
  render: () => <SearchBox />,
};

export default meta;
