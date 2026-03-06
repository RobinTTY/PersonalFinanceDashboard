import { useState } from 'react';
import { Button } from '@mantine/core';
import { ComponentPreview } from '../storybook/ComponentPreview';
import attributes from './attributes.json';
import { SettingsModal } from './SettingsModal';

export default { title: 'Settings/SettingsModal' };

export function Usage() {
  const [opened, setOpened] = useState(false);

  return (
    <ComponentPreview canvas={attributes.canvas} withSpacing>
      <Button onClick={() => setOpened(true)}>Open Settings</Button>
      <SettingsModal opened={opened} onClose={() => setOpened(false)} />
    </ComponentPreview>
  );
}

export function OpenByDefault() {
  const [opened, setOpened] = useState(true);

  return (
    <ComponentPreview canvas={attributes.canvas} withSpacing>
      <Button onClick={() => setOpened(true)}>Open Settings</Button>
      <SettingsModal opened={opened} onClose={() => setOpened(false)} />
    </ComponentPreview>
  );
}
