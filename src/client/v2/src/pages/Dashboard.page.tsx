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
        min: (value: { min: number; max: number }) => {
          const padding = (value.max - value.min) * 0.1;
          const paddedMin = value.min - padding;
          return Math.floor(paddedMin / 1000) * 1000;
        },
        max: (value: { min: number; max: number }) => {
          return Math.ceil(value.max / 1000) * 1000;
        },
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
