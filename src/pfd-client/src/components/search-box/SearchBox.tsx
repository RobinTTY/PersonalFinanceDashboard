import {
  TextInput,
  TextInputProps,
  ActionIcon,
  useMantineTheme,
} from "@mantine/core";
import { IconSearch, IconArrowRight, IconArrowLeft } from "@tabler/icons-react";

export const SearchBox = (props: SearchBoxProps) => {
  const theme = useMantineTheme();
  const { actionIconActive, ...textInputProps } = props;

  return (
    <TextInput
      icon={<IconSearch size="1.1rem" stroke={1.5} />}
      radius="xl"
      size="md"
      rightSection={
        actionIconActive ? (
          <ActionIcon
            size={32}
            radius="xl"
            color={theme.primaryColor}
            variant="filled"
          >
            {theme.dir === "ltr" ? (
              <IconArrowRight size="1.1rem" stroke={1.5} />
            ) : (
              <IconArrowLeft size="1.1rem" stroke={1.5} />
            )}
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