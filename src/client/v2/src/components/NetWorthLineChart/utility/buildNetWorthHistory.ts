/**
 * The most days the net worth history will span. One year of daily points keeps the
 * series bounded while the full-date labels (e.g. "Oct 3, 2025") stay unique within a
 * single window. ECharts thins out overlapping x-axis labels automatically, so every
 * day is plotted even though only a subset of labels are drawn.
 */
const MAX_DAYS = 365;

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
  /** Reconstructed end-of-day balance keyed by full date label (e.g. "Oct 3, 2025"). */
  dataByLabel: Record<string, number>;
  /** The currency shared by most accounts, used for display. */
  currency: string | null;
}

/**
 * Reconstructs the combined available balance across all accounts for each day.
 *
 * The backend only stores the *current* balance per account, so historical values
 * are derived by walking the transaction history backwards: the balance at a past
 * point in time equals the current total minus the net of every transaction that
 * happened after that point. Transactions from all accounts are summed together,
 * which assumes a single currency (the common case) — see {@link pickDominantCurrency}.
 *
 * @param accounts The accounts to combine, each with its current balance and transactions.
 * @param now Reference point for "today"; injectable to keep the output deterministic in tests.
 * @returns The day-keyed balance series and the dominant currency.
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

  const todayStart = startOfDay(now);
  const capDayStart = new Date(now.getFullYear(), now.getMonth(), now.getDate() - (MAX_DAYS - 1));

  // Start at the day of the earliest transaction so we don't fabricate a flat
  // pre-history, but never reach further back than MAX_DAYS.
  let startDayStart = todayStart;

  if (transactions.length > 0) {
    const earliest = transactions.reduce(
      (min, transaction) => (transaction.date < min ? transaction.date : min),
      transactions[0].date
    );

    const earliestDayStart = startOfDay(earliest);
    startDayStart = earliestDayStart < capDayStart ? capDayStart : earliestDayStart;
  }

  const dataByLabel: Record<string, number> = {};
  for (
    let cursor = startDayStart;
    cursor <= todayStart;
    cursor = new Date(cursor.getFullYear(), cursor.getMonth(), cursor.getDate() + 1)
  ) {
    const isToday = cursor.getTime() === todayStart.getTime();
    // For past days use the last instant of the day; for today use `now` so the
    // final point matches the true current balance exactly.
    const boundary = isToday
      ? now
      : new Date(cursor.getFullYear(), cursor.getMonth(), cursor.getDate(), 23, 59, 59, 999);
    const netAfterBoundary = transactions.reduce(
      (sum, transaction) => (transaction.date > boundary ? sum + transaction.amount : sum),
      0
    );
    const label = cursor.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    });
    dataByLabel[label] = round2(totalNow - netAfterBoundary);
  }

  return { dataByLabel, currency };
}

function startOfDay(value: Date): Date {
  return new Date(value.getFullYear(), value.getMonth(), value.getDate());
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
