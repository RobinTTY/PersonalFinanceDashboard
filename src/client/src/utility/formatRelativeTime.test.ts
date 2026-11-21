import { formatRelativeTime } from './formatRelativeTime';

const now = new Date('2026-11-21T12:00:00Z');
const minutesFromNow = (minutes: number) => new Date(now.getTime() + minutes * 60 * 1000);

describe('formatRelativeTime', () => {
  it.each([
    [0, 'just now'],
    [-0.5, 'just now'],
    [-5, '5 minutes ago'],
    [-90, '2 hours ago'],
    [-24 * 60, 'yesterday'],
    [-3 * 24 * 60, '3 days ago'],
    [120, 'in 2 hours'],
  ])('formats %d minutes from now as "%s"', (minutes, expected) => {
    expect(formatRelativeTime(minutesFromNow(minutes), now)).toBe(expected);
  });
});
