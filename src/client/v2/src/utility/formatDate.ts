export function formatDate(date: Date | null | undefined): string {
  if (!date) {
    return '—';
  }
  return date.toLocaleDateString('en-US', {
    day: '2-digit',
    month: 'short',
    year: 'numeric',
  });
}
