/**
 * The most months the net worth history will span. Keeping this at 12 keeps the
 * x-axis readable and guarantees the short month labels (e.g. "Oct") stay unique
 * within a single window.
 */
const MAX_MONTHS = 12;

interface NetWorthTransaction {
  valueDate?: Date | string | null;
  amount: number;
}

export interface NetWorthAccount {
  balance?: number | null;
  currency?: string | null;
  transactions: NetWorthTransaction[];
}

export interface NetWorthHistory {
  /** Reconstructed end-of-month balance keyed by short month label. */
  dataByLabel: Record<string, number>;
  /** The currency shared by most accounts, used for display. */
  currency: string | null;
}

/**
 * Reconstructs the combined available balance across all accounts for each month.
 *
 * The backend only stores the *current* balance per account, so historical values
 * are derived by walking the transaction history backwards: the balance at a past
 * point in time equals the current total minus the net of every transaction that
 * happened after that point. Transactions from all accounts are summed together,
 * which assumes a single currency (the common case) — see {@link pickDominantCurrency}.
 *
 * @param accounts The accounts to combine, each with its current balance and transactions.
 * @param now Reference point for "today"; injectable to keep the output deterministic in tests.
 * @returns The month-keyed balance series and the dominant currency.
 */
export function buildNetWorthHistory(
  accounts: NetWorthAccount[],
  now: Date = new Date()
): NetWorthHistory {
  const totalNow = accounts.reduce((sum, account) => sum + (account.balance ?? 0), 0);

  const transactions = accounts
    .flatMap((account) => account.transactions)
    .map((transaction) => ({ date: toDate(transaction.valueDate), amount: transaction.amount }))
    .filter(
      (transaction): transaction is { date: Date; amount: number } => transaction.date != null
    );

  const currency = pickDominantCurrency(accounts);

  const currentMonthStart = new Date(now.getFullYear(), now.getMonth(), 1);
  const capMonthStart = new Date(now.getFullYear(), now.getMonth() - (MAX_MONTHS - 1), 1);

  // Start at the month of the earliest transaction so we don't fabricate a flat
  // pre-history, but never reach further back than MAX_MONTHS.
  let startMonthStart = currentMonthStart;

  if (transactions.length > 0) {
    const earliest = transactions.reduce(
      (min, transaction) => (transaction.date < min ? transaction.date : min),
      transactions[0].date
    );

    const earliestMonthStart = new Date(earliest.getFullYear(), earliest.getMonth(), 1);
    startMonthStart = earliestMonthStart < capMonthStart ? capMonthStart : earliestMonthStart;
  }

  const dataByLabel: Record<string, number> = {};
  for (
    let cursor = startMonthStart;
    cursor <= currentMonthStart;
    cursor = new Date(cursor.getFullYear(), cursor.getMonth() + 1, 1)
  ) {
    const isCurrentMonth =
      cursor.getFullYear() === now.getFullYear() && cursor.getMonth() === now.getMonth();
    // For past months use the last instant of the month; for the current month use
    // `now` so the final point matches the true current balance exactly.
    const boundary = isCurrentMonth
      ? now
      : new Date(cursor.getFullYear(), cursor.getMonth() + 1, 0, 23, 59, 59, 999);
    const netAfterBoundary = transactions.reduce(
      (sum, transaction) => (transaction.date > boundary ? sum + transaction.amount : sum),
      0
    );
    const label = cursor.toLocaleDateString('en-US', { month: 'short' });
    dataByLabel[label] = round2(totalNow - netAfterBoundary);
  }

  return { dataByLabel, currency };
}

function toDate(value: Date | string | null | undefined): Date | null {
  if (value == null) {
    return null;
  }
  const date = value instanceof Date ? value : new Date(value);
  return Number.isNaN(date.getTime()) ? null : date;
}

function round2(value: number): number {
  return Math.round(value * 100) / 100;
}

function pickDominantCurrency(accounts: NetWorthAccount[]): string | null {
  const counts = new Map<string, number>();
  for (const account of accounts) {
    if (account.currency) {
      counts.set(account.currency, (counts.get(account.currency) ?? 0) + 1);
    }
  }

  let dominant: string | null = null;
  let dominantCount = 0;

  for (const [currency, count] of counts) {
    if (count > dominantCount) {
      dominant = currency;
      dominantCount = count;
    }
  }

  return dominant;
}
