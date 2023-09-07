import {
  UnstyledButton,
  Center,
  Text,
  Group,
  createStyles,
  rem,
} from "@mantine/core";
import { ModalButtonProps } from "./ModalButtonProps";
import { IconChevronRight } from "@tabler/icons-react";

const useStyles = createStyles((theme) => ({
  button: {
    display: "block",
    width: "100%",
    border: "solid .5px #404040",
    padding: theme.spacing.xl,
    borderRadius: theme.radius.sm,
    color: theme.colorScheme === "dark" ? theme.colors.dark[0] : theme.black,
    backgroundColor:
      theme.colorScheme === "dark" ? theme.colors.dark[6] : theme.white,
    "&:hover": {
      border: "solid .5px #808080",
      backgroundColor:
        theme.colorScheme === "dark"
          ? theme.colors.dark[4]
          : theme.colors.gray[0],
    },
  },

  iconContainer: {
    "> *": {
      height: "inherit",
    },
  },
}));

// TODO: Light mode styling
/**
 * The available options when adding an account.
 * @param param0 The button icon and text.
 * @returns A button with the icon and text.
 */
export const ModalButton = ({
  icon,
  iconPosition,
  iconHeight,
  description,
  includeChevron,
  padding,
  action,
}: ModalButtonProps) => {
  const { classes } = useStyles();

  // TODO: implement position right, bottom
  const flagAndText = (
    <>
      <Center h={iconHeight} className={classes.iconContainer}>
        {icon}
      </Center>
      <Center>
        <Text fz="lg" fw={700}>
          {description}
        </Text>
      </Center>
    </>
  );

  const flagAndTextLayout =
    iconPosition === "left" ? <Group>{flagAndText}</Group> : flagAndText;

  const chevronContent = (
    <Group w="100%" position="apart">
      {flagAndTextLayout}
      <IconChevronRight size={rem(28)} />
    </Group>
  );

  const content =
    includeChevron && iconPosition === "left" ? chevronContent : flagAndText;

  return (
    <UnstyledButton onClick={action} className={classes.button} p={padding}>
      {content}
    </UnstyledButton>
  );
};
