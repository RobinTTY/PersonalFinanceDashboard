import { IconChevronRight } from '@tabler/icons-react';
import { Avatar, Group, Text, UnstyledButton } from '@mantine/core';
import classes from './UserButton.module.css';

interface UserButtonProps {
  onClick?: () => void;
}

export function UserButton({ onClick }: UserButtonProps) {
  return (
    <UnstyledButton className={classes.user} onClick={onClick}>
      <Group>
        <Avatar
          src="https://raw.githubusercontent.com/mantinedev/mantine/master/.demo/avatars/avatar-8.png"
          radius="xl"
          alt="Harriette Spoonlicker"
        />

        <div style={{ flex: 1 }}>
          <Text size="sm" fw={500}>
            Harriette Spoonlicker
          </Text>

          <Text c="dimmed" size="xs">
            hspoonlicker@outlook.com
          </Text>
        </div>

        <IconChevronRight size={14} stroke={1.5} />
      </Group>
    </UnstyledButton>
  );
}
