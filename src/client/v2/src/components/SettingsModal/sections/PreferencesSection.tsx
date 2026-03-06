import { IconDeviceDesktop, IconMoon, IconSun } from '@tabler/icons-react';
import { Group, SegmentedControl, Stack, Text } from '@mantine/core';
import { useMantineColorScheme } from '@mantine/core';
import type { MantineColorScheme } from '@mantine/core';

const themeOptions: { value: MantineColorScheme; label: React.ReactNode }[] = [
  {
    value: 'light',
    label: (
      <Group gap={4} wrap="nowrap">
        <IconSun size={14} />
        <span>Light</span>
      </Group>
    ),
  },
  {
    value: 'dark',
    label: (
      <Group gap={4} wrap="nowrap">
        <IconMoon size={14} />
        <span>Dark</span>
      </Group>
    ),
  },
  {
    value: 'auto',
    label: (
      <Group gap={4} wrap="nowrap">
        <IconDeviceDesktop size={14} />
        <span>System</span>
      </Group>
    ),
  },
];

export function PreferencesSection() {
  const { colorScheme, setColorScheme } = useMantineColorScheme();

  return (
    <Stack gap="xl">
      <Group justify="space-between" align="center">
        <Stack gap={2}>
          <Text fw={500} size="sm">
            Theme
          </Text>
          <Text size="xs" c="dimmed">
            Choose between light, dark, or follow your system setting.
          </Text>
        </Stack>
        <SegmentedControl
          value={colorScheme}
          onChange={(value) => setColorScheme(value as MantineColorScheme)}
          data={themeOptions}
        />
      </Group>
    </Stack>
  );
}
