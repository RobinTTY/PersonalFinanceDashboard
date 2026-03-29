import { IconChevronRight } from '@tabler/icons-react';
import { Avatar, Group, Text, UnstyledButton } from '@mantine/core';
import classes from './SelectionCard.module.css';

interface SelectionCardProps {
  id: string;
  optionName: string;
  logoUri?: string;
  isSelected: boolean;
  onSelect: (id: string) => void;
}

export function SelectionCard({ id, optionName: name, logoUri, isSelected, onSelect }: SelectionCardProps) {
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

/**
 * Utility function to get initials from a name string. Takes the first letter of the first two words.
 * @param name The name string to extract initials from.
 * @returns The initials in uppercase.
 */
function getInitials(name: string): string {
  return name
    .split(' ')
    .slice(0, 2)
    .map((word) => word[0])
    .join('')
    .toUpperCase();
}
