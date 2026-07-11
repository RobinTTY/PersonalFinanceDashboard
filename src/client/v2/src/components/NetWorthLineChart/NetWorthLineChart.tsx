import { useEffect, useRef } from 'react';
import { useComputedColorScheme } from '@mantine/core';
import * as echarts from 'echarts';

interface NetWorthLineChartProps {
  dataByLabel: Record<string, number>;
  seriesName?: string;
  height?: number;
  /** ISO currency code used to format tooltip values (e.g. "EUR"). */
  currency?: string | null;
}

/**
 * Per-theme palette. Colors are validated against the chart surface for the
 * lightness band and ≥3:1 contrast (see the dataviz palette validator).
 */
const PALETTES = {
  light: {
    accent: '#1c7ed6',
    areaTop: 'rgba(28, 126, 214, 0.22)',
    areaBottom: 'rgba(28, 126, 214, 0)',
    grid: 'rgba(0, 0, 0, 0.06)',
    axisLabel: '#868e96',
    surface: '#ffffff',
    tooltipBg: '#ffffff',
    tooltipBorder: '#e9ecef',
    inkStrong: '#212529',
    inkMuted: '#868e96',
  },
  dark: {
    accent: '#339af0',
    areaTop: 'rgba(51, 154, 240, 0.30)',
    areaBottom: 'rgba(51, 154, 240, 0)',
    grid: 'rgba(255, 255, 255, 0.06)',
    axisLabel: '#909296',
    surface: '#25262b',
    tooltipBg: '#25262b',
    tooltipBorder: '#373a40',
    inkStrong: '#f1f3f5',
    inkMuted: '#909296',
  },
} as const;

export function NetWorthLineChart({
  dataByLabel,
  seriesName = 'Combined Accounts',
  height = 320,
  currency,
}: NetWorthLineChartProps) {
  const chartContainerRef = useRef<HTMLDivElement | null>(null);
  const colorScheme = useComputedColorScheme('light');

  useEffect(() => {
    const entries = Object.entries(dataByLabel);

    if (!chartContainerRef.current || entries.length === 0) {
      return;
    }

    const palette = PALETTES[colorScheme];
    const labels = entries.map(([label]) => label);
    const data = entries.map(([, value]) => value);

    const minDataPoint = Math.min(...data);
    const maxDataPoint = Math.max(...data);
    const yAxisPadding = (maxDataPoint - minDataPoint) * 0.1;
    const yAxisMin = Math.floor((minDataPoint - yAxisPadding) / 1000) * 1000;
    const roughInterval = (maxDataPoint - yAxisMin) / 4;
    const yAxisInterval = Math.max(500, Math.ceil(roughInterval / 500) * 500);
    const yAxisMax = yAxisMin + yAxisInterval * 4;

    const currencyFormatter = new Intl.NumberFormat(undefined, {
      style: currency ? 'currency' : 'decimal',
      currency: currency ?? undefined,
      maximumFractionDigits: 2,
    });
    const compactFormatter = new Intl.NumberFormat(undefined, {
      notation: 'compact',
      maximumFractionDigits: 1,
    });

    const chart = echarts.init(chartContainerRef.current, undefined, {
      renderer: 'svg',
    });

    chart.setOption({
      grid: { left: 8, right: 16, top: 16, bottom: 8, containLabel: true },
      tooltip: {
        trigger: 'axis',
        backgroundColor: palette.tooltipBg,
        borderColor: palette.tooltipBorder,
        borderWidth: 1,
        padding: [8, 12],
        extraCssText: 'border-radius: 10px; box-shadow: 0 8px 24px rgba(0, 0, 0, 0.18);',
        axisPointer: {
          type: 'line',
          lineStyle: { color: palette.accent, width: 1, type: 'dashed', opacity: 0.6 },
        },
        formatter: (params: unknown) => {
          const point = (Array.isArray(params) ? params[0] : params) as {
            axisValueLabel?: string;
            name?: string;
            value: number;
          };
          const label = point.axisValueLabel ?? point.name ?? '';
          const value = currencyFormatter.format(point.value);
          return `
            <div style="font-weight:600;color:${palette.inkStrong};margin-bottom:4px">${label}</div>
            <div style="display:flex;align-items:center;gap:8px;white-space:nowrap">
              <span style="width:8px;height:8px;border-radius:50%;background:${palette.accent}"></span>
              <span style="color:${palette.inkMuted}">${seriesName}</span>
              <span style="margin-left:16px;font-weight:600;color:${palette.inkStrong}">${value}</span>
            </div>`;
        },
      },
      xAxis: {
        type: 'category',
        data: labels,
        boundaryGap: false,
        axisLine: { show: false },
        axisTick: { show: false },
        axisLabel: {
          color: palette.axisLabel,
          fontSize: 11,
          margin: 12,
          hideOverlap: true,
        },
      },
      yAxis: {
        type: 'value',
        scale: true,
        min: yAxisMin,
        max: yAxisMax,
        interval: yAxisInterval,
        splitNumber: 4,
        axisLine: { show: false },
        axisTick: { show: false },
        axisLabel: {
          color: palette.axisLabel,
          fontSize: 11,
          margin: 12,
          formatter: (value: number) => compactFormatter.format(value),
        },
        splitLine: {
          lineStyle: { color: palette.grid, type: 'dashed' },
        },
      },
      series: [
        {
          name: seriesName,
          type: 'line',
          smooth: true,
          showSymbol: false,
          symbol: 'circle',
          symbolSize: 8,
          sampling: 'lttb',
          lineStyle: {
            width: 2,
            color: palette.accent,
            shadowColor: palette.areaTop,
            shadowBlur: 12,
            shadowOffsetY: 6,
          },
          itemStyle: {
            color: palette.accent,
            borderColor: palette.surface,
            borderWidth: 2,
          },
          areaStyle: {
            color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
              { offset: 0, color: palette.areaTop },
              { offset: 1, color: palette.areaBottom },
            ]),
          },
          emphasis: { focus: 'series', scale: 1.4 },
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
  }, [dataByLabel, seriesName, height, currency, colorScheme]);

  return <div ref={chartContainerRef} style={{ width: '100%', height }} />;
}
