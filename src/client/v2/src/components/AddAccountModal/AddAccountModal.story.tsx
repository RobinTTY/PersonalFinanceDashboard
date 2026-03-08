import { useState } from 'react';
import { Button } from '@mantine/core';
import { ComponentPreview } from '../storybook/ComponentPreview';
import { AddAccountModal } from './AddAccountModal';

export default { title: 'Accounts/AddAccountModal' };

export function Usage() {
  const [opened, setOpened] = useState(false);

  return (
    <ComponentPreview canvas={{ center: true }} withSpacing>
      <Button onClick={() => setOpened(true)}>Add Account</Button>
      <AddAccountModal opened={opened} onClose={() => setOpened(false)} />
    </ComponentPreview>
  );
}

export function OpenByDefault() {
  const [opened, setOpened] = useState(true);

  return (
    <ComponentPreview canvas={{ center: true }} withSpacing>
      <Button onClick={() => setOpened(true)}>Add Account</Button>
      <AddAccountModal opened={opened} onClose={() => setOpened(false)} />
    </ComponentPreview>
  );
}
