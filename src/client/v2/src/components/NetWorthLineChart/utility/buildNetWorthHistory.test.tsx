import { buildNetWorthHistory, NetWorthAccount } from './buildNetWorthHistory';

// Fixed reference "now" so day labels and reconstructed values are deterministic.
const NOW = new Date(2026, 2, 20); // 20 Mar 2026

describe('buildNetWorthHistory', () => {
  it('reconstructs end-of-day balances backwards from the current balance', () => {
    const accounts: NetWorthAccount[] = [
      {
        balance: 1000,
        currency: 'EUR',
        transactions: [
          { valueDate: new Date(2026, 0, 15), amount: 100 }, // Jan 15: +100
          { valueDate: new Date(2026, 1, 10), amount: -50 }, // Feb 10: -50
          { valueDate: new Date(2026, 2, 5), amount: 200 }, // Mar 5: +200
        ],
      },
    ];

    const { dataByLabel, currency } = buildNetWorthHistory(accounts, NOW);
    const labels = Object.keys(dataByLabel);

    expect(currency).toBe('EUR');
    // The window runs daily from the earliest transaction's day (Jan 15) through today
    // (Mar 20): 17 days in Jan + 28 in Feb + 20 in Mar = 65 points.
    expect(labels).toHaveLength(65);
    expect(labels[0]).toBe('Jan 15, 2026');
    expect(labels[labels.length - 1]).toBe('Mar 20, 2026');

    // Current total is 1000; working backwards through the transactions:
    //   Mar 20 (today) = 1000
    //   Feb 10 (day of the -50) = 1000 - 200 (Mar txn) = 800
    //   Jan 15 (day of the +100) = 800 - (-50) (Feb txn) = 850
    expect(dataByLabel['Jan 15, 2026']).toBe(850);
    expect(dataByLabel['Feb 10, 2026']).toBe(800);
    expect(dataByLabel['Mar 20, 2026']).toBe(1000);
    // The +200 lands on Mar 5, so the day before is still 800.
    expect(dataByLabel['Mar 4, 2026']).toBe(800);
    expect(dataByLabel['Mar 5, 2026']).toBe(1000);
  });

  it('combines balances and transactions across multiple accounts', () => {
    const accounts: NetWorthAccount[] = [
      {
        balance: 500,
        currency: 'EUR',
        transactions: [
          { valueDate: new Date(2026, 1, 5), amount: 40 }, // Feb 5: +40
          { valueDate: new Date(2026, 2, 3), amount: 100 }, // Mar 3: +100
        ],
      },
      {
        balance: 300,
        currency: 'EUR',
        transactions: [{ valueDate: new Date(2026, 2, 8), amount: -60 }], // Mar 8: -60
      },
    ];

    const { dataByLabel } = buildNetWorthHistory(accounts, NOW);

    // Combined current total: 800. The window starts at the earliest transaction day
    // (Feb 5). End of Feb 5 = 800 - (100 - 60) [later March txns] = 760.
    expect(Object.keys(dataByLabel)[0]).toBe('Feb 5, 2026');
    expect(dataByLabel['Feb 5, 2026']).toBe(760);
    expect(dataByLabel['Mar 20, 2026']).toBe(800);
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
          { valueDate: new Date(2026, 1, 12), amount: -30 }, // Feb 12: -30
          { valueDate: new Date(2026, 2, 5), amount: 200 }, // Mar 5: +200
        ],
      },
    ];

    const { dataByLabel } = buildNetWorthHistory(accounts, NOW);
    // The invalid-date transaction (999) is ignored entirely. Window starts at the
    // earliest valid transaction day (Feb 12).
    expect(Object.keys(dataByLabel)[0]).toBe('Feb 12, 2026');
    expect(dataByLabel['Feb 12, 2026']).toBe(800); // 1000 - 200 (Mar txn)
    expect(dataByLabel['Mar 20, 2026']).toBe(1000);
  });

  it('handles accounts with no transactions as a single current-balance point', () => {
    const accounts: NetWorthAccount[] = [{ balance: 1234.5, currency: 'EUR', transactions: [] }];
    const { dataByLabel } = buildNetWorthHistory(accounts, NOW);
    expect(dataByLabel).toEqual({ 'Mar 20, 2026': 1234.5 });
  });

  it('caps the window at the most recent year of daily points', () => {
    const accounts: NetWorthAccount[] = [
      {
        balance: 1000,
        currency: 'EUR',
        transactions: [
          { valueDate: new Date(2020, 0, 1), amount: 10 }, // far older than the cap
        ],
      },
    ];

    const { dataByLabel } = buildNetWorthHistory(accounts, NOW);
    const labels = Object.keys(dataByLabel);

    expect(labels).toHaveLength(365);
    expect(labels[labels.length - 1]).toBe('Mar 20, 2026');
    expect(labels[0]).toBe('Mar 21, 2025'); // 365 days back, inclusive
  });
});
