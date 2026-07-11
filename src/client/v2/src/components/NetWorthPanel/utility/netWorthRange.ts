export type RangeKey = '1M' | '3M' | '6M' | '1Y';

/**
 * Number of trailing daily points each range keeps. The series is one point per
 * day (see {@link buildNetWorthHistory}), so slicing the last N entries is the
 * same as keeping the last N days. "1Y" (365) covers the full capped series.
 */
const RANGE_DAYS: Record<RangeKey, number> = {
  '1M': 30,
  '3M': 90,
  '6M': 180,
  '1Y': 365,
};

export const RANGE_OPTIONS: { label: string; value: RangeKey }[] = [
  { label: '1M', value: '1M' },
  { label: '3M', value: '3M' },
  { label: '6M', value: '6M' },
  { label: '1Y', value: '1Y' },
];

export const RANGE_LABELS: Record<RangeKey, string> = {
  '1M': 'Past month',
  '3M': 'Past 3 months',
  '6M': 'Past 6 months',
  '1Y': 'Past year',
};

/** Direction of a change, used to pick color and iconography. */
export type ChangeDirection = 'up' | 'down' | 'flat';

export interface NetWorthChange {
  /** The most recent (current) balance. */
  current: number;
  /** Absolute change across the window (current − first). */
  change: number;
  /** Percentage change relative to the first point, or null when it is zero. */
  changePct: number | null;
  direction: ChangeDirection;
}

/**
 * Keeps only the trailing entries for the given range. Entry order is chronological
 * (insertion order), so this yields the most recent `RANGE_DAYS[range]` days; when
 * fewer days exist, the whole series is returned.
 */
export function sliceRange(
  dataByLabel: Record<string, number>,
  range: RangeKey
): Record<string, number> {
  const entries = Object.entries(dataByLabel);
  return Object.fromEntries(entries.slice(-RANGE_DAYS[range]));
}

/**
 * Summarizes a series into the current value and its change since the first point.
 * The percentage is relative to |first| so the sign reflects the direction of the
 * change even when the starting balance is negative.
 */
export function computeChange(dataByLabel: Record<string, number>): NetWorthChange {
  const values = Object.values(dataByLabel);

  if (values.length === 0) {
    return { current: 0, change: 0, changePct: null, direction: 'flat' };
  }

  const current = values[values.length - 1];
  const first = values[0];
  const change = round2(current - first);
  const changePct = first !== 0 ? round2((change / Math.abs(first)) * 100) : null;
  const direction: ChangeDirection = change > 0 ? 'up' : change < 0 ? 'down' : 'flat';

  return { current, change, changePct, direction };
}

/** Formats a value as currency when a code is supplied, otherwise as a plain number. */
export function formatCurrency(
  value: number,
  currency: string | null | undefined,
  options?: Intl.NumberFormatOptions
): string {
  return new Intl.NumberFormat(undefined, {
    style: currency ? 'currency' : 'decimal',
    currency: currency ?? undefined,
    maximumFractionDigits: 2,
    ...options,
  }).format(value);
}

function round2(value: number): number {
  return Math.round(value * 100) / 100;
}
