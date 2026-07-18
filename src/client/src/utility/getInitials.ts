/**
 * Utility function to get initials from a name string. Takes the first letter of the first two words.
 * @param name The name string to extract initials from.
 * @returns The initials in uppercase.
 */
export function getInitials(name: string): string {
  return name
    .split(' ')
    .slice(0, 2)
    .map((word) => word[0])
    .join('')
    .toUpperCase();
}
