import type { Meta, StoryObj } from '@storybook/react';
import { GB } from 'country-flag-icons/react/3x2';
import { ModalButton } from './ModalButton';

const meta: Meta<typeof ModalButton> = {
  component: ModalButton,
  title: 'Components/ModalButton',
  tags: ['autodocs'],
};

type Story = StoryObj<typeof ModalButton>;

// TODO: This component is not reusable yet, layout (iconLeft) only applied if includeChevron is true
// TODO: ModalButton and AccountButton could be combined into one component
export const Default: Story = {
  render: () => (
    <ModalButton
      icon={<GB />}
      description="Great Britain"
      iconHeight="32px"
      iconWidth="32px"
      iconPosition="left"
      textWidth={300}
      includeChevron
    />
  ),
};

export default meta;
