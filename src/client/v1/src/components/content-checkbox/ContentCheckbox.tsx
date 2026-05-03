import { UnstyledButton, Checkbox, Box } from '@mantine/core';
import { ContentCheckboxProps } from './ContentCheckboxProps';

// TODO: remove unnecessary css
import classes from './ContentCheckbox.module.css';

// TODO: Can this be simplified so onChange is not necessary?
export const ContentCheckbox = ({
  className: _className,
  children,
  checkboxProps,
  onChange,
  ...others
}: ContentCheckboxProps) => (
  <UnstyledButton
    onClick={() => onChange?.(!checkboxProps.checked)}
    data-checked={checkboxProps.checked || undefined}
    className={classes.button}
    {...others}
  >
    <Box className={classes.body}>{children}</Box>
    <Checkbox
      styles={{ input: { cursor: 'pointer' } }}
      tabIndex={-1}
      size="md"
      pl="xl"
      {...checkboxProps}
      onChange={() => {}}
    />
  </UnstyledButton>
);
