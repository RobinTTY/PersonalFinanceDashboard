import { useState } from 'react';
import { IconChevronRight } from '@tabler/icons-react';
import { NavLink, useLocation, useNavigate } from 'react-router-dom';
import { Box, Collapse, Group, Text, ThemeIcon, UnstyledButton } from '@mantine/core';
import classes from './NavbarLinksGroup.module.css';

export interface LinksGroupProps {
  icon: React.FC<any>;
  label: string;
  link?: string;
  initiallyOpened?: boolean;
  links?: { label: string; link: string }[];
  onClick?: () => void;
}

export function LinksGroup({
  icon: Icon,
  label,
  link,
  initiallyOpened,
  links,
  onClick,
}: LinksGroupProps) {
  const hasLinks = Array.isArray(links);
  const [opened, setOpened] = useState(initiallyOpened || false);
  const navigate = useNavigate();
  const location = useLocation();
  const active = link
    ? location.pathname === link || location.pathname.startsWith(`${link}/`)
    : false;

  const items = (hasLinks ? links : []).map((item) => (
    <Text<typeof NavLink>
      component={NavLink}
      className={classes.link}
      to={item.link}
      key={item.label}
    >
      {item.label}
    </Text>
  ));

  const handleClick = () => {
    if (hasLinks) {
      setOpened((o) => !o);
    } else if (onClick) {
      onClick();
    } else if (link) {
      navigate(link);
    }
  };

  return (
    <>
      <UnstyledButton
        onClick={handleClick}
        className={classes.control}
        data-active={active || undefined}
      >
        <Group justify="space-between" gap={0}>
          <Box style={{ display: 'flex', alignItems: 'center' }}>
            <ThemeIcon
              variant={active ? 'gradient' : 'light'}
              gradient={{ from: 'blue', to: 'cyan', deg: 135 }}
              size={40}
              radius="md"
            >
              <Icon size={24} stroke={1.7} />
            </ThemeIcon>
            <Box ml="md">{label}</Box>
          </Box>
          {hasLinks && (
            <IconChevronRight
              className={classes.chevron}
              stroke={1.5}
              size={16}
              style={{ transform: opened ? 'rotate(-90deg)' : 'none' }}
            />
          )}
        </Group>
      </UnstyledButton>
      {hasLinks ? <Collapse expanded={opened}>{items}</Collapse> : null}
    </>
  );
}

export function NavbarLinksGroup(props: LinksGroupProps) {
  return (
    <Box mih={220} p="md">
      <LinksGroup {...props} />
    </Box>
  );
}
