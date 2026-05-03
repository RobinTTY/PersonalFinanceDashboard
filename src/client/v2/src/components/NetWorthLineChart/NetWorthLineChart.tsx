import { useEffect, useRef } from 'react';
import * as echarts from 'echarts';

interface NetWorthLineChartProps {
  dataByLabel: Record<string, number>;
  seriesName?: string;
  height?: number;
}

export function NetWorthLineChart({
  dataByLabel,
  seriesName = 'Combined Accounts',
  height = 320,
}: NetWorthLineChartProps) {
  const chartContainerRef = useRef<HTMLDivElement | null>(null);

  useEffect(() => {
    const entries = Object.entries(dataByLabel);

    if (!chartContainerRef.current || entries.length === 0) {
      return;
    }

    const labels = entries.map(([label]) => label);
    const data = entries.map(([, value]) => value);

    const minDataPoint = Math.min(...data);
    const maxDataPoint = Math.max(...data);
    const yAxisPadding = (maxDataPoint - minDataPoint) * 0.1;
    const yAxisMin = Math.floor((minDataPoint - yAxisPadding) / 1000) * 1000;
    const roughInterval = (maxDataPoint - yAxisMin) / 4;
    const yAxisInterval = Math.max(500, Math.ceil(roughInterval / 500) * 500);
    const yAxisMax = yAxisMin + yAxisInterval * 4;

    const chart = echarts.init(chartContainerRef.current);

    chart.setOption({
      grid: {
        left: 48,
        right: 24,
        top: 24,
        bottom: 24,
        containLabel: false,
      },
      tooltip: {
        trigger: 'axis',
      },
      xAxis: {
        type: 'category',
        data: labels,
        boundaryGap: false,
      },
      yAxis: {
        type: 'value',
        scale: true,
        min: yAxisMin,
        max: yAxisMax,
        interval: yAxisInterval,
        splitNumber: 4,
        axisLabel: {
          margin: 16,
        },
      },
      series: [
        {
          name: seriesName,
          type: 'line',
          smooth: true,
          data,
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
  }, [dataByLabel, seriesName]);

  return <div ref={chartContainerRef} style={{ width: '100%', height }} />;
}
