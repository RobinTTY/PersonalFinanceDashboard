import { Card, Stack, Text, Title } from '@mantine/core';
import { useEffect, useRef } from 'react';
import * as echarts from 'echarts';

export function DashboardPage() {
  const chartContainerRef = useRef<HTMLDivElement | null>(null);
  const accountBalanceData = [12400, 12950, 13100, 13820, 14210, 14940];

  useEffect(() => {
    if (!chartContainerRef.current) {
      return;
    }

    // Lowest value in the plotted dataset
    const minDataPoint = Math.min(...accountBalanceData);
    // Highest value in the plotted dataset
    const maxDataPoint = Math.max(...accountBalanceData);
    // Extra room so the line does not touch chart bounds
    const yAxisPadding = (maxDataPoint - minDataPoint) * 0.1;
    // Rounded-down start of the y-axis to full 1,000s.
    const yAxisMin = Math.floor((minDataPoint - yAxisPadding) / 1000) * 1000;
    // Target size of each of the 4 y-axis segments (5 grid lines total).
    const roughInterval = (maxDataPoint - yAxisMin) / 4;
    // Rounded interval for cleaner axis labels and grid spacing.
    const yAxisInterval = Math.ceil(roughInterval / 500) * 500;
    // End of the y-axis based on 4 intervals above the computed minimum.
    const yAxisMax = yAxisMin + yAxisInterval * 4;

    const chart = echarts.init(chartContainerRef.current);

    chart.setOption({
      tooltip: {
        trigger: 'axis',
      },
      xAxis: {
        type: 'category',
        data: ['Oct', 'Nov', 'Dec', 'Jan', 'Feb', 'Mar'],
      },
      yAxis: {
        type: 'value',
        scale: true,
        min: yAxisMin,
        max: yAxisMax,
        interval: yAxisInterval,
        splitNumber: 4,
      },
      series: [
        {
          name: 'Combined Accounts',
          type: 'line',
          smooth: true,
          data: accountBalanceData,
        },
      ],
    });

    const handleResize = () => {
      chart.resize();
    };

    window.addEventListener('resize', handleResize);

    return () => {
      window.removeEventListener('resize', handleResize);
      chart.dispose();
    };
  }, []);

  return (
    <Stack>
      <Title order={2}>Dashboard</Title>

      <Card withBorder>
        <Stack gap="xs">
          <Text fw={600}>Net Worth</Text>
          <Text size="sm" c="dimmed">
            Total Accounts Balance Over Time
          </Text>
          <div ref={chartContainerRef} style={{ width: '100%', height: 320 }} />
        </Stack>
      </Card>
    </Stack>
  );
}
