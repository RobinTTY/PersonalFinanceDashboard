type BankAccount = {
  balance: number | null;
  currency: string | null;
};

/**
 * Utility function to format the balance of a connected bank account for display in the UI.
 * If the balance is null or undefined, it returns null. If a currency is provided, it formats
 * the balance as "amount currency". If no currency is provided, it returns just the amount as a string.
 * @param account The bank account for which to format the balance.
 * @returns The formatted balance or null if the balance is not available.
 */
export function formatBalance(account: BankAccount): string | null {
  if (account.balance == null) {
    return null;
  }
  return account.currency
    ? `${String(account.balance)} ${account.currency}`
    : String(account.balance);
}