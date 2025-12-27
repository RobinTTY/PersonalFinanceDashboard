import type { Meta, StoryObj } from '@storybook/react-vite';
import { ContentCheckbox } from './ContentCheckbox';

const meta: Meta<typeof ContentCheckbox> = {
  component: ContentCheckbox,
  title: 'Components/ContentCheckbox',
  tags: ['autodocs'],
};

type Story = StoryObj<typeof ContentCheckbox>;

export const Default: Story = {
  render: () => <ContentCheckbox checkboxProps={{}}>Test</ContentCheckbox>,
};

export default meta;
