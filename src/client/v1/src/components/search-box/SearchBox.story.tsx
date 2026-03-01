import type { Meta, StoryObj } from '@storybook/react-vite';
import { SearchBox } from './SearchBox';

const meta: Meta<typeof SearchBox> = {
  component: SearchBox,
  title: 'Components/SearchBox',
  tags: ['autodocs'],
};

type Story = StoryObj<typeof SearchBox>;

export const Default: Story = {
  render: () => <SearchBox />,
};

export default meta;
