import { StyleProp } from '@mantine/core';
import { CSSProperties } from 'react';

export interface ModalOptionSearchListProps {
  options: Array<Option>;
  displayOptions?: ModalOptionSearchDisplayOptions;
  truncateOptionDescription?: boolean;
  optionDescriptionWidth?: number;
  searchPlaceholder?: string;
  onOptionSelect?: (optionKey: string) => void;
}

export interface Option {
  key: string;
  description: string;
  icon: React.ReactElement;
}

export interface ModalOptionSearchDisplayOptions {
  iconHeight?: StyleProp<CSSProperties['height']>;
  iconWidth?: StyleProp<CSSProperties['width']>;
}

export interface RowProps {
  index: number;
  style: React.CSSProperties;
}
