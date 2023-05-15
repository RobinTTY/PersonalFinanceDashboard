import { Group, Text, ActionIcon, useMantineColorScheme } from "@mantine/core";
import { IconSun, IconMoonStars } from "@tabler/icons-react";

const AppHeader = () => {
  const { colorScheme, toggleColorScheme } = useMantineColorScheme();

  // TODO: reconsider spacing units (rem, 40...)
  return (
    <Group position="apart" style={{ width: "100%" }}>
      <Text>Application Name</Text>
      <ActionIcon
        variant="default"
        onClick={() => toggleColorScheme()}
        size={40}
      >
        {colorScheme === "dark" ? (
          <IconSun size="1.3rem" />
        ) : (
          <IconMoonStars size="1.3rem" />
        )}
      </ActionIcon>
    </Group>
  );
};
export default AppHeader;
