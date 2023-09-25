import { PropsWithChildren } from 'react';
import { UnstyledButton, Checkbox, Box } from '@mantine/core';
import { useUncontrolled } from '@mantine/hooks';
import { ContentCheckboxProps } from './ContentCheckboxProps';

// TODO: remove unnecessary css
import classes from './ContentCheckbox.module.css';

export const ContentCheckbox = ({
  checked,
  defaultChecked,
  onChange,
  className,
  children,
  ...others
}: PropsWithChildren<ContentCheckboxProps> &
  Omit<React.ComponentPropsWithoutRef<'button'>, keyof ContentCheckboxProps>) => {
  const [value, handleChange] = useUncontrolled({
    value: checked,
    defaultValue: defaultChecked,
    finalValue: false,
    onChange,
  });

  return (
    <UnstyledButton
      {...others}
      onClick={() => handleChange(!value)}
      data-checked={value || undefined}
      className={classes.button}
    >
      <Box className={classes.body}>{children}</Box>
      <Checkbox
        checked={value}
        onChange={() => {}}
        tabIndex={-1}
        size="md"
        styles={{ input: { cursor: 'pointer' } }}
        pl="xl"
      />
    </UnstyledButton>
  );
};
