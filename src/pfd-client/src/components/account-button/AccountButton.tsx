import { UnstyledButton, Group, Avatar, Text, Box, useMantineTheme, rem } from '@mantine/core';
import { IconChevronRight } from '@tabler/icons-react';

import classes from './AccountButton.module.css';

export const AccountButton = () => {
  return (
    <Box className={classes['container-box']}>
      <UnstyledButton className={classes['button']}>
        <Group justify="space-between">
          <Group>
            <Avatar
              src="https://images.unsplash.com/photo-1500648767791-00dcc994a43e?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=255&q=80"
              radius="xl"
            />
            <Box className={classes.userBox}>
              <Text size="sm" fw={500}>
                Jurica Koletić
              </Text>
              <Text c="dimmed" size="xs" fw={500}>
                jkol@gmail.com
              </Text>
            </Box>
          </Group>
          {/* TODO: is this the correct way to size icons now? */}
          <IconChevronRight style={{ height: rem(18) }} />
        </Group>
      </UnstyledButton>
    </Box>
  );
};
