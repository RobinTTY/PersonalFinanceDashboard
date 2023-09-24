import { IconChevronRight } from '@tabler/icons-react';
import { UnstyledButton, Center, Text, Group, rem } from '@mantine/core';
import { ModalButtonProps } from './ModalButtonProps';

import classes from './ModalButton.module.css';

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
  truncateDescription,
  textWidth,
  includeChevron,
  padding,
  action,
}: ModalButtonProps) => {
  // TODO: implement position right, bottom
  // TODO: truncate should be based on parent container width?
  const flagAndText = (
    <>
      <Center h={iconHeight} className={classes['icon-container']}>
        {icon}
      </Center>
      <Center>
        {truncateDescription ? (
          <Text fz="lg" fw={700} w={textWidth} truncate>
            {description}
          </Text>
        ) : (
          <Text fz="lg" fw={700}>
            {description}
          </Text>
        )}
      </Center>
    </>
  );

  const flagAndTextLayout = iconPosition === 'left' ? <Group>{flagAndText}</Group> : flagAndText;

  const chevronContent = (
    <Group w="100%" justify="space-between">
      {flagAndTextLayout}
      <IconChevronRight style={{ height: rem(28) }} />
    </Group>
  );

  const content = includeChevron && iconPosition === 'left' ? chevronContent : flagAndText;

  return (
    <UnstyledButton onClick={action} className={classes.button} p={padding}>
      {content}
    </UnstyledButton>
  );
};
