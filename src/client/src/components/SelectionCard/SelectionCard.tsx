import { IconChevronRight } from '@tabler/icons-react';
import { Avatar, Group, Stack, Text, UnstyledButton } from '@mantine/core';
import { getInitials } from '@/utility/getInitials';
import classes from './SelectionCard.module.css';

interface SelectionCardProps {
  id: string;
  optionName: string;
  logoUri?: string;
  /**
   * Optional secondary content rendered beneath the option name, e.g. metadata
   * that distinguishes otherwise similarly named options.
   */
  subtitle?: React.ReactNode;
  isSelected: boolean;
  onSelect: (id: string) => void;
}

export function SelectionCard({
  id,
  optionName: name,
  logoUri,
  subtitle,
  isSelected,
  onSelect,
}: SelectionCardProps) {
  return (
    <UnstyledButton
      className={classes.selectionCard}
      data-selected={isSelected || undefined}
      onClick={() => onSelect(id)}
      style={{ minWidth: 0 }}
    >
      <Group justify="space-between" wrap="nowrap">
        <Group wrap="nowrap" gap="sm" style={{ minWidth: 0 }}>
          <Avatar src={logoUri} radius="sm" size="md" flex="0 0 auto">
            {getInitials(name)}
          </Avatar>
          <Stack gap={3} style={{ minWidth: 0 }}>
            <Text fw={500} size="sm" truncate>
              {name}
            </Text>
            {subtitle}
          </Stack>
        </Group>
        <IconChevronRight size={16} style={{ flexShrink: 0 }} />
      </Group>
    </UnstyledButton>
  );
}
