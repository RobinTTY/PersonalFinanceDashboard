/**
 * Formats a date in the user's locale, e.g. "Jan 5, 2026", or returns "—" without a date.
 */
export function formatDate(date: Date | null | undefined): string {
  if (!date) {
    return '—';
  }
  return date.toLocaleDateString(undefined, {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  });
}
