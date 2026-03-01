import { PropsWithChildren } from 'react';
import { CheckboxProps } from '@mantine/core';

export interface PartialContentCheckboxProps {
  checkboxProps: CheckboxProps;
  onChange?: (checked: boolean) => void;
}

export type ContentCheckboxProps = PropsWithChildren<PartialContentCheckboxProps> &
  Omit<React.ComponentPropsWithoutRef<'button'>, keyof PartialContentCheckboxProps>;
