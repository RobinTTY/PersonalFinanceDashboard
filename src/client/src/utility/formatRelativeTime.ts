const UNITS: [Intl.RelativeTimeFormatUnit, number][] = [
  ['year', 365 * 24 * 60 * 60],
  ['month', 30 * 24 * 60 * 60],
  ['week', 7 * 24 * 60 * 60],
  ['day', 24 * 60 * 60],
  ['hour', 60 * 60],
  ['minute', 60],
];

const formatter = new Intl.RelativeTimeFormat('en-US', { numeric: 'auto' });

/**
 * Formats a date relative to now, e.g. "2 hours ago" or "in 3 days".
 */
export function formatRelativeTime(date: Date, now: Date = new Date()): string {
  const seconds = (date.getTime() - now.getTime()) / 1000;
  for (const [unit, unitSeconds] of UNITS) {
    if (Math.abs(seconds) >= unitSeconds) {
      // Rounded away from zero on halves in both directions (Math.round rounds -1.5 to -1)
      const value = Math.sign(seconds) * Math.round(Math.abs(seconds) / unitSeconds);
      return formatter.format(value, unit);
    }
  }
  return 'just now';
}
