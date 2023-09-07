import { UnstyledButton, Center, Text } from "@mantine/core";
import { ModalButtonProps } from "./ModalButtonProps";

// TODO: Light mode styling
/**
 * The available options when adding an account.
 * @param param0 The button icon and text.
 * @returns A button with the icon and text.
 */
export const ModalButton = ({
  icon,
  iconPosition,
  iconSize,
  description,
  action,
}: ModalButtonProps) => {
  return (
    <UnstyledButton
      onClick={action}
      sx={(theme) => ({
        display: "block",
        width: "100%",
        border: "solid .5px #404040",
        padding: theme.spacing.xl,
        borderRadius: theme.radius.sm,
        color:
          theme.colorScheme === "dark" ? theme.colors.dark[0] : theme.black,
        backgroundColor:
          theme.colorScheme === "dark" ? theme.colors.dark[6] : theme.white,
        "&:hover": {
          border: "solid .5px #808080",
          backgroundColor:
            theme.colorScheme === "dark"
              ? theme.colors.dark[4]
              : theme.colors.gray[0],
        },
      })}
    >
      <Center>{icon}</Center>
      <Center>
        <Text fz="lg" fw={700}>
          {description}
        </Text>
      </Center>
    </UnstyledButton>
  );
};
