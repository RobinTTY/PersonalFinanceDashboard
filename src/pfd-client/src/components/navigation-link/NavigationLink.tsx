import { ThemeIcon, UnstyledButton, Group, Text } from '@mantine/core';

import classes from './NavigationLink.module.css';

export const NavigationLink = ({ icon, color, label }: NavigationLinkProps) => {
  return (
    <UnstyledButton className={classes['unstyled-button']}>
      <Group>
        <ThemeIcon size="lg" color={color} variant="light">
          {icon}
        </ThemeIcon>

        <Text size="lg" fw="500">
          {label}
        </Text>
      </Group>
    </UnstyledButton>
  );
};

interface NavigationLinkProps {
  icon: React.ReactNode;
  color: string;
  label: string;
}
