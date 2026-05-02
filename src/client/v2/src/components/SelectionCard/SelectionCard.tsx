import { IconChevronRight } from '@tabler/icons-react';
import { Avatar, Group, Text, UnstyledButton } from '@mantine/core';
import { getInitials } from '@/utility/getInitials';
import classes from './SelectionCard.module.css';

interface SelectionCardProps {
  id: string;
  optionName: string;
  logoUri?: string;
  isSelected: boolean;
  onSelect: (id: string) => void;
}

export function SelectionCard({
  id,
  optionName: name,
  logoUri,
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
          <Text fw={500} size="sm" truncate>
            {name}
          </Text>
        </Group>
        <IconChevronRight size={16} style={{ flexShrink: 0 }} />
      </Group>
    </UnstyledButton>
  );
}
