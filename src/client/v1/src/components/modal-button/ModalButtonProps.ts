import React from 'react';
import { StyleProp, MantineSpacing } from '@mantine/core';
import { CSSProperties } from 'react';

export interface ModalButtonProps {
  icon: React.ReactElement;
  iconPosition?: 'left' | 'right' | 'top' | 'bottom';
  iconHeight?: StyleProp<CSSProperties['height']>;
  iconWidth?: StyleProp<CSSProperties['width']>;
  description: string;
  truncateDescription?: boolean;
  textWidth?: number;
  includeChevron?: boolean;
  padding?: StyleProp<MantineSpacing>;
  action?: () => void;
}
