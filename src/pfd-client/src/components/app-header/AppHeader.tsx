import { Group, Text, ActionIcon, useMantineColorScheme } from "@mantine/core";
import { IconSun, IconMoonStars } from "@tabler/icons-react";

export const AppHeader = () => {
  const { colorScheme, toggleColorScheme } = useMantineColorScheme();

  // TODO: reconsider spacing units (rem, 40...)
  return (
    <Group position="apart" style={{ width: "100%" }}>
      <Text>Personal Finance Dashboard</Text>
      <ActionIcon
        variant="default"
        onClick={() => toggleColorScheme()}
        size={35}
      >
        {colorScheme === "dark" ? (
          <IconSun size="1.2rem" />
        ) : (
          <IconMoonStars size="1.2rem" />
        )}
      </ActionIcon>
    </Group>
  );
};
