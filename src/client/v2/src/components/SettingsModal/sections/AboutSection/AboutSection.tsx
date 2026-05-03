import { IconBrandGithub, IconBug, IconFileText, IconUser } from '@tabler/icons-react';
import { Anchor, Code, Divider, Group, Stack, Text, ThemeIcon } from '@mantine/core';

interface InfoRowProps {
  icon: React.ReactNode;
  label: string;
  children: React.ReactNode;
}

function InfoRow({ icon, label, children }: InfoRowProps) {
  return (
    <Group justify="space-between" align="center">
      <Group gap="xs">
        <ThemeIcon variant="light" size="md" color="gray">
          {icon}
        </ThemeIcon>
        <Text size="sm" fw={500}>
          {label}
        </Text>
      </Group>
      {children}
    </Group>
  );
}

export function AboutSection() {
  const iconSize = 20;

  return (
    <Stack gap="lg">
      <Stack gap={4}>
        <Text fw={600} size="md">
          {import.meta.env.VITE_APP_TITLE}
        </Text>
        <Group gap="xs" align="center">
          <Text size="sm" c="dimmed">
            Version
          </Text>
          <Code>{import.meta.env.VITE_APP_VERSION}</Code>
        </Group>
        <Text size="xs" c="dimmed">
          A self-hosted dashboard to track your bank accounts, transactions and stock portfolio.
        </Text>
      </Stack>

      <Divider />

      <Stack gap="sm">
        <InfoRow icon={<IconBrandGithub size={iconSize} />} label="Repository">
          <Anchor
            href="https://github.com/RobinTTY/PersonalFinanceDashboard"
            target="_blank"
            rel="noopener noreferrer"
            size="sm"
          >
            RobinTTY/PersonalFinanceDashboard
          </Anchor>
        </InfoRow>

        <InfoRow icon={<IconUser size={iconSize} />} label="Developer">
          <Anchor
            href="https://github.com/RobinTTY"
            target="_blank"
            rel="noopener noreferrer"
            size="sm"
          >
            github.com/RobinTTY
          </Anchor>
        </InfoRow>

        <InfoRow icon={<IconFileText size={iconSize} />} label="License">
          <Anchor
            href="https://github.com/RobinTTY/PersonalFinanceDashboard/blob/main/LICENSE.md"
            target="_blank"
            rel="noopener noreferrer"
            size="sm"
          >
            View License
          </Anchor>
        </InfoRow>

        <InfoRow icon={<IconBug size={iconSize} />} label="Report a Bug">
          <Anchor
            href="https://github.com/RobinTTY/PersonalFinanceDashboard/issues"
            target="_blank"
            rel="noopener noreferrer"
            size="sm"
          >
            GitHub Issues
          </Anchor>
        </InfoRow>
      </Stack>
    </Stack>
  );
}
