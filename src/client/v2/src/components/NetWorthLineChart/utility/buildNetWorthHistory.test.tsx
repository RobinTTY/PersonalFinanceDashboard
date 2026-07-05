import { buildNetWorthHistory, NetWorthAccount } from './buildNetWorthHistory';

// Fixed reference "now" so month labels and reconstructed values are deterministic.
const NOW = new Date(2026, 2, 20); // 20 Mar 2026

describe('buildNetWorthHistory', () => {
  it('reconstructs month-end balances backwards from the current balance', () => {
    const accounts: NetWorthAccount[] = [
      {
        balance: 1000,
        currency: 'EUR',
        transactions: [
          { valueDate: new Date(2026, 0, 15), amount: 100 }, // Jan: +100
          { valueDate: new Date(2026, 1, 10), amount: -50 }, // Feb: -50
          { valueDate: new Date(2026, 2, 5), amount: 200 }, // Mar: +200
        ],
      },
    ];

    const { dataByLabel, currency } = buildNetWorthHistory(accounts, NOW);

    expect(currency).toBe('EUR');
    // Window starts at the earliest transaction's month (Jan). Current total is
    // 1000; working backwards from the month boundaries:
    //   Mar (current month) = 1000
    //   end of Feb = 1000 - 200 (Mar txn) = 800
    //   end of Jan = 800 - (-50) (Feb txn) = 850
    expect(dataByLabel).toEqual({
      Jan: 850,
      Feb: 800,
      Mar: 1000,
    });
  });

  it('combines balances and transactions across multiple accounts', () => {
    const accounts: NetWorthAccount[] = [
      {
        balance: 500,
        currency: 'EUR',
        transactions: [
          { valueDate: new Date(2026, 1, 5), amount: 40 }, // Feb: +40
          { valueDate: new Date(2026, 2, 3), amount: 100 }, // Mar: +100
        ],
      },
      {
        balance: 300,
        currency: 'EUR',
        transactions: [{ valueDate: new Date(2026, 2, 8), amount: -60 }], // Mar: -60
      },
    ];

    const { dataByLabel } = buildNetWorthHistory(accounts, NOW);

    // Combined current total: 800. The window starts at the earliest transaction
    // month (Feb). End of Feb = 800 - (100 - 60) [March txns] = 760.
    expect(dataByLabel).toEqual({ Feb: 760, Mar: 800 });
  });

  it('picks the dominant currency when accounts differ', () => {
    const accounts: NetWorthAccount[] = [
      { balance: 100, currency: 'EUR', transactions: [] },
      { balance: 100, currency: 'EUR', transactions: [] },
      { balance: 100, currency: 'USD', transactions: [] },
    ];

    expect(buildNetWorthHistory(accounts, NOW).currency).toBe('EUR');
  });

  it('ignores transactions without a valid date', () => {
    const accounts: NetWorthAccount[] = [
      {
        balance: 1000,
        currency: 'EUR',
        transactions: [
          { valueDate: null, amount: 999 },
          { valueDate: new Date(2026, 1, 12), amount: -30 }, // Feb: -30
          { valueDate: new Date(2026, 2, 5), amount: 200 }, // Mar: +200
        ],
      },
    ];

    const { dataByLabel } = buildNetWorthHistory(accounts, NOW);
    // The invalid-date transaction (999) is ignored entirely. Window starts at the
    // earliest valid transaction month (Feb).
    expect(dataByLabel).toEqual({ Feb: 800, Mar: 1000 }); // Feb = 1000 - 200 (Mar txn)
  });

  it('handles accounts with no transactions as a flat current balance', () => {
    const accounts: NetWorthAccount[] = [{ balance: 1234.5, currency: 'EUR', transactions: [] }];
    const { dataByLabel } = buildNetWorthHistory(accounts, NOW);
    expect(dataByLabel).toEqual({ Mar: 1234.5 });
  });
});
