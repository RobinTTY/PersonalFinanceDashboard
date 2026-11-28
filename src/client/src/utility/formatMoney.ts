/**
 * Formats an amount of money in the user's locale, e.g. "€1,234.50". Without a currency, the amount is formatted
 * as a plain number with two decimals.
 * @param value The amount to format.
 * @param currency The ISO 4217 code of the amount's currency.
 * @param options Further options, e.g. `signDisplay`, which override the defaults.
 */
export function formatMoney(
  value: number,
  currency: string | null | undefined,
  options?: Intl.NumberFormatOptions
): string {
  const plain = { minimumFractionDigits: 2, maximumFractionDigits: 2, ...options };
  if (!currency) {
    return new Intl.NumberFormat(undefined, plain).format(value);
  }
  try {
    return new Intl.NumberFormat(undefined, { style: 'currency', currency, ...options }).format(
      value
    );
  } catch (error) {
    // Intl only accepts well-formed currency codes
    if (error instanceof RangeError) {
      return `${new Intl.NumberFormat(undefined, plain).format(value)} ${currency}`;
    }
    throw error;
  }
}
