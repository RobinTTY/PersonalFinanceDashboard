import { useEffect, useRef } from 'react';
import { Text, Paper } from '@mantine/core';
import { type ECharts, init, getInstanceByDom } from 'echarts';
import { EChartProps } from './EChartProps';

import './LineGraph.css';

// TODO: use mantine theme for dark/light mode
export const LineGraph = ({ option, style, settings, loading, theme }: EChartProps) => {
  // const theme = useMantineTheme();
  const chartRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    // Initialize chart
    let chart: ECharts | undefined;
    if (chartRef.current !== null) {
      // TODO: canvas renderer looks blurry, why? Probably resizing issue
      chart = init(chartRef.current, theme, { renderer: 'canvas' });
    }

    // TODO: ResizeObserver is leading to a bit janky UX
    function resizeChart() {
      chart?.resize();
    }
    window.addEventListener('resize', resizeChart);

    // Return cleanup function
    return () => {
      chart?.dispose();
      window.removeEventListener('resize', resizeChart);
    };
  }, [theme]);

  useEffect(() => {
    if (chartRef.current !== null) {
      const chart = getInstanceByDom(chartRef.current);
      chart?.setOption(option, settings);
    }
  }, [option, settings, theme]); // Whenever theme changes we need to add options and settings due to it being deleted in cleanup function

  useEffect(() => {
    if (chartRef.current !== null) {
      const chart = getInstanceByDom(chartRef.current);
      loading === true ? chart?.showLoading() : chart?.hideLoading();
    }
  }, [loading, theme]);

  return (
    <Paper withBorder p="md" radius="md" h="50%">
      <div id="main-container">
        <Text id="graph-title" size="lg" pl="xl" c="white">
          Net Worth
        </Text>
        <div id="graph">
          <div ref={chartRef} style={{ width: '100%', height: '100%', ...style }} />;
        </div>
      </div>
    </Paper>
  );
};
