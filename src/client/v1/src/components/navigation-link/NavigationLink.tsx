import { ThemeIcon, UnstyledButton, Group, Text } from '@mantine/core';
import { Link } from 'react-router-dom';

import classes from './NavigationLink.module.css';

export const NavigationLink = ({ icon, color, label, to }: NavigationLinkProps) => (
  <UnstyledButton className={classes['unstyled-button']} component={Link} to={to}>
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
interface NavigationLinkProps {
  icon: React.ReactNode;
  color: string;
  label: string;
  to: string;
}
