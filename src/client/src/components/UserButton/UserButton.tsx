import { IconChevronRight } from '@tabler/icons-react';
import { Avatar, Group, Text, UnstyledButton } from '@mantine/core';
import { useCurrentUser } from '@/hooks/useCurrentUser';
import classes from './UserButton.module.css';

interface UserButtonProps {
  onClick?: () => void;
}

export function UserButton({ onClick }: UserButtonProps) {
  const { name, email, avatarUrl } = useCurrentUser();

  return (
    <UnstyledButton className={classes.user} onClick={onClick}>
      <Group>
        <Avatar src={avatarUrl} radius="xl" alt={name} name={name} color="initials" />

        <div style={{ flex: 1 }}>
          <Text size="sm" fw={500}>
            {name}
          </Text>

          {email && (
            <Text c="dimmed" size="xs">
              {email}
            </Text>
          )}
        </div>

        <IconChevronRight size={14} stroke={1.5} />
      </Group>
    </UnstyledButton>
  );
}
