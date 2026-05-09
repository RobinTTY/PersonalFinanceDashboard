export function formatAmount(amount: number, currency: string): string {
  const sign = amount >= 0 ? '+' : '−';
  return `${sign}${Math.abs(amount).toFixed(2)} ${currency}`;
}
