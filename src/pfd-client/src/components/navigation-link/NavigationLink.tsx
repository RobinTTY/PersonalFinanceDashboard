import { ThemeIcon, UnstyledButton, Group, Text } from "@mantine/core";

const NavigationLink = ({ icon, color, label }: NavigationLinkProps) => {
  return (
    <UnstyledButton
      sx={(theme) => ({
        display: "block",
        width: "100%",
        padding: theme.spacing.xs,
        borderRadius: theme.radius.sm,
        color:
          theme.colorScheme === "dark" ? theme.colors.dark[0] : theme.black,
        "&:hover": {
          backgroundColor:
            theme.colorScheme === "dark"
              ? theme.colors.dark[6]
              : theme.colors.gray[0],
        },
      })}
    >
      <Group>
        <ThemeIcon size="lg" color={color} variant="light">
          {icon}
        </ThemeIcon>

        <Text size="lg">{label}</Text>
      </Group>
    </UnstyledButton>
  );
};

export default NavigationLink;

interface NavigationLinkProps {
  icon: React.ReactNode;
  color: string;
  label: string;
}
