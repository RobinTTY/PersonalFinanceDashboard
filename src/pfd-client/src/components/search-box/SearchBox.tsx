import { TextInput, TextInputProps, ActionIcon } from '@mantine/core';
import { IconSearch, IconArrowRight } from '@tabler/icons-react';

export const SearchBox = (props: SearchBoxProps) => {
  const { actionIconActive, ...textInputProps } = props;

  return (
    <TextInput
      leftSection={<IconSearch size="1.1rem" stroke={1.5} />}
      radius="xl"
      size="md"
      rightSection={
        actionIconActive ? (
          <ActionIcon size={32} radius="xl" variant="filled">
            <IconArrowRight size="1.1rem" stroke={1.5} />
          </ActionIcon>
        ) : null
      }
      placeholder="Search for something..."
      rightSectionWidth={42}
      {...textInputProps}
    />
  );
};

interface SearchBoxProps extends TextInputProps {
  actionIconActive?: boolean;
}
