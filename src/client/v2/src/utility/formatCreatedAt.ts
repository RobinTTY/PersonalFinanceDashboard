/**
 * Formats the createdAt value of a bank account for display in the UI. If the value is a valid date, it formats it as "MMM D, YYYY".
 * If it's not a valid date, it returns the original value as a string. If the value is falsy, it returns an empty string.
 * @param value The createdAt value to format, which can be of any type.
 * @returns The formatted createdAt value.
 */
export function formatCreatedAt(value: unknown): string {
  if (!value) {
    return '';
  }
  const date = new Date(String(value));
  if (Number.isNaN(date.getTime())) {
    return String(value);
  }
  return date.toLocaleDateString(undefined, { year: 'numeric', month: 'short', day: 'numeric' });
}