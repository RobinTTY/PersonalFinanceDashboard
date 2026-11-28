import { formatMoney } from './formatMoney';

// Expectations are built with Intl, so they hold in the locale of any test runner
describe('formatMoney', () => {
  it('formats amounts in their currency', () => {
    const expected = new Intl.NumberFormat(undefined, {
      style: 'currency',
      currency: 'EUR',
    }).format(1234.5);

    expect(formatMoney(1234.5, 'EUR')).toBe(expected);
  });

  it('formats amounts without a currency as numbers with two decimals', () => {
    const expected = new Intl.NumberFormat(undefined, { minimumFractionDigits: 2 }).format(1000);

    expect(formatMoney(1000, null)).toBe(expected);
  });

  it('appends currencies Intl does not accept', () => {
    const expected = new Intl.NumberFormat(undefined, { minimumFractionDigits: 2 }).format(5);

    expect(formatMoney(5, 'EURO')).toBe(`${expected} EURO`);
  });

  it('applies the given options', () => {
    expect(formatMoney(50, 'EUR', { signDisplay: 'exceptZero' })).toMatch(/^\+/);
    expect(formatMoney(50, null, { signDisplay: 'exceptZero' })).toMatch(/^\+/);
  });
});
