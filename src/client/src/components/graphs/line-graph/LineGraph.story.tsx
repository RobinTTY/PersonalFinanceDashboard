import type { Meta, StoryObj } from '@storybook/react';
import { LineGraph } from './LineGraph';
import { echartOptions } from './SampleLineData';

const meta: Meta<typeof LineGraph> = {
  component: LineGraph,
  title: 'Components/LineGraph',
  tags: ['autodocs'],
};

type Story = StoryObj<typeof LineGraph>;

export const Default: Story = {
  render: () => (
    <div style={{ height: '95vh' }}>
      <LineGraph option={echartOptions} />
    </div>
  ),
};

export default meta;
