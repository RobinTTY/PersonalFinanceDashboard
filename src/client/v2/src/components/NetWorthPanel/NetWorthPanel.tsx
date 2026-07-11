import { useMemo, useState } from 'react';
import { IconArrowDownRight, IconArrowUpRight, IconMinus } from '@tabler/icons-react';
import { Badge, Group, SegmentedControl, Stack, Text } from '@mantine/core';
import { NetWorthLineChart } from '@/components/NetWorthLineChart/NetWorthLineChart';
import {
  computeChange,
  formatCurrency,
  RANGE_LABELS,
  RANGE_OPTIONS,
  RangeKey,
  sliceRange,
} from './utility/netWorthRange';

interface NetWorthPanelProps {
  /** Full daily balance series keyed by date label. */
  dataByLabel: Record<string, number>;
  currency?: string | null;
  height?: number;
  defaultRange?: RangeKey;
}

const DIRECTION = {
  up: { color: 'teal', Icon: IconArrowUpRight },
  down: { color: 'red', Icon: IconArrowDownRight },
  flat: { color: 'gray', Icon: IconMinus },
} as const;

/** Small uppercase label shown above the balance; shared with the card's other states. */
export function NetWorthLabel() {
  return (
    <Text size="sm" fw={600} tt="uppercase" c="dimmed" style={{ letterSpacing: '0.05em' }}>
      Net Worth
    </Text>
  );
}

export function NetWorthPanel({
  dataByLabel,
  currency,
  height = 320,
  defaultRange = '3M',
}: NetWorthPanelProps) {
  const [range, setRange] = useState<RangeKey>(defaultRange);

  const visible = useMemo(() => sliceRange(dataByLabel, range), [dataByLabel, range]);
  const { current, change, changePct, direction } = useMemo(
    () => computeChange(visible),
    [visible]
  );

  const { color, Icon } = DIRECTION[direction];
  const pctLabel = changePct == null ? '—' : `${formatSigned(changePct)}%`;
  const absChange = formatCurrency(change, currency, { signDisplay: 'exceptZero' });

  return (
    <Stack gap="lg">
      <Group justify="space-between" align="flex-start" wrap="nowrap">
        <Stack gap={6}>
          <NetWorthLabel />
          <Text fz={32} fw={700} lh={1.15}>
            {formatCurrency(current, currency)}
          </Text>
          <Group gap="xs" align="center">
            <Badge
              color={color}
              variant="light"
              radius="sm"
              size="lg"
              leftSection={<Icon size={14} stroke={2.5} />}
            >
              {pctLabel}
            </Badge>
            <Text size="sm" c="dimmed">
              {absChange} · {RANGE_LABELS[range]}
            </Text>
          </Group>
        </Stack>

        <SegmentedControl
          size="sm"
          radius="md"
          value={range}
          onChange={(value) => setRange(value as RangeKey)}
          data={RANGE_OPTIONS}
        />
      </Group>

      <NetWorthLineChart dataByLabel={visible} currency={currency} height={height} />
    </Stack>
  );
}

function formatSigned(value: number): string {
  return new Intl.NumberFormat(undefined, {
    maximumFractionDigits: 1,
    signDisplay: 'exceptZero',
  }).format(value);
}
