import type { Meta, StoryObj } from '@storybook/react-vite';
import { CZ, DK, FI, FR, GB, IT, SE } from 'country-flag-icons/react/3x2';
import { ModalOptionSearchList } from './ModalOptionSearchList';

const meta: Meta<typeof ModalOptionSearchList> = {
  component: ModalOptionSearchList,
  title: 'Components/ModalOptionSearchList',
  tags: ['autodocs'],
};

type Story = StoryObj<typeof ModalOptionSearchList>;

// TODO: padding should be on parent container not here!
export const Default: Story = {
  render: () => (
    <ModalOptionSearchList
      options={[
        { key: 'GB', description: 'Great Britain', icon: <GB /> },
        { key: 'DK', description: 'Denmark', icon: <DK /> },
        { key: 'CZ', description: 'Czech Republic', icon: <CZ /> },
        { key: 'FR', description: 'France', icon: <FR /> },
        { key: 'FI', description: 'Finland', icon: <FI /> },
        { key: 'SE', description: 'Sweden', icon: <SE /> },
        { key: 'IT', description: 'Italy', icon: <IT /> },
      ]}
    />
  ),
};

export default meta;
