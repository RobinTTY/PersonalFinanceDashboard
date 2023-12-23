import type { CSSProperties } from 'react';
import type { EChartsOption, SetOptionOpts } from 'echarts';

export interface EChartProps {
  option: EChartsOption;
  style?: CSSProperties;
  settings?: SetOptionOpts;
  loading?: boolean;
  theme?: 'light' | 'dark';
}
