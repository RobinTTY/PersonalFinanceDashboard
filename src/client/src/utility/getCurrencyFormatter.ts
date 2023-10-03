/**
 * Returns a currency formatter for the given options.
 * Reference: https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Intl/NumberFormat
 * @param locale The locale to use.
 * @param currency The currency to use.
 * @param minFractionDigits The minimum number of fraction digits to use.
 * @param maxFractionDigits The maximum number of fraction digits to use.
 * @returns A currency formatter with the given options.
 */
export const getCurrencyFormatter = (
  locale: string,
  currency: string,
  minFractionDigits?: number,
  maxFractionDigits?: number
) =>
  new Intl.NumberFormat(locale, {
    style: 'currency',
    currency,
    minimumFractionDigits: minFractionDigits, // (this suffices for whole numbers, but will print 2500.10 as $2,500.1)
    maximumFractionDigits: maxFractionDigits, // (causes 2500.99 to be printed as $2,501)
  });
