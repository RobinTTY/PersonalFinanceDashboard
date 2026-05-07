import { useState } from 'react';
import { Group, Select, Stack, Table, Text, Title } from '@mantine/core';

interface Transaction {
  id: string;
  valueDate: string;
  amount: number;
  payer: string;
  payee: string;
  currency: string;
  accountId: string;
}

const DEMO_ACCOUNTS = [
  { value: 'acc-1', label: 'Main Checking' },
  { value: 'acc-2', label: 'Savings Account' },
  { value: 'acc-3', label: 'Credit Card' },
];

const DEMO_TRANSACTIONS: Transaction[] = [
  { id: '1', valueDate: '2026-05-06', amount: -89.5, payer: 'Main Checking', payee: 'Amazon', currency: 'EUR', accountId: 'acc-1' },
  { id: '2', valueDate: '2026-05-05', amount: 2400.0, payer: 'Employer GmbH', payee: 'Main Checking', currency: 'EUR', accountId: 'acc-1' },
  { id: '3', valueDate: '2026-05-04', amount: -1200.0, payer: 'Main Checking', payee: 'Landlord', currency: 'EUR', accountId: 'acc-1' },
  { id: '4', valueDate: '2026-05-04', amount: -45.99, payer: 'Credit Card', payee: 'Netflix', currency: 'EUR', accountId: 'acc-3' },
  { id: '5', valueDate: '2026-05-03', amount: 500.0, payer: 'Main Checking', payee: 'Savings Account', currency: 'EUR', accountId: 'acc-1' },
  { id: '6', valueDate: '2026-05-03', amount: 500.0, payer: 'Main Checking', payee: 'Savings Account', currency: 'EUR', accountId: 'acc-2' },
  { id: '7', valueDate: '2026-05-02', amount: -32.8, payer: 'Credit Card', payee: 'Supermarket', currency: 'EUR', accountId: 'acc-3' },
  { id: '8', valueDate: '2026-05-01', amount: -120.0, payer: 'Main Checking', payee: 'Gym Membership', currency: 'EUR', accountId: 'acc-1' },
  { id: '9', valueDate: '2026-04-30', amount: 150.0, payer: 'Freelance Client', payee: 'Main Checking', currency: 'USD', accountId: 'acc-1' },
  { id: '10', valueDate: '2026-04-29', amount: -19.99, payer: 'Credit Card', payee: 'Spotify', currency: 'EUR', accountId: 'acc-3' },
];

function formatAmount(amount: number, currency: string): string {
  const sign = amount >= 0 ? '+' : '-';
  return `${sign}${Math.abs(amount).toFixed(2)} ${currency}`;
}

function formatDate(dateStr: string): string {
  return new Date(dateStr).toLocaleDateString('en-US', {
    day: '2-digit',
    month: 'short',
    year: 'numeric',
  });
}

export function TransactionsPage() {
  const [selectedAccount, setSelectedAccount] = useState<string | null>(null);

  const transactions = selectedAccount
    ? DEMO_TRANSACTIONS.filter((t) => t.accountId === selectedAccount)
    : DEMO_TRANSACTIONS;

  const rows = transactions.map((transaction) => (
    <Table.Tr key={transaction.id}>
      <Table.Td>
        <Text size="sm">{formatDate(transaction.valueDate)}</Text>
      </Table.Td>
      <Table.Td>
        <Text size="sm">{transaction.payer}</Text>
      </Table.Td>
      <Table.Td>
        <Text size="sm">{transaction.payee}</Text>
      </Table.Td>
      <Table.Td style={{ textAlign: 'right' }}>
        <Text size="sm" fw={600} c={transaction.amount >= 0 ? 'green' : 'red'}>
          {formatAmount(transaction.amount, transaction.currency)}
        </Text>
      </Table.Td>
    </Table.Tr>
  ));

  return (
    <Stack>
      <Group justify="space-between" align="flex-end">
        <Title order={2} m={0}>
          Transactions
        </Title>
        <Select
          placeholder="All accounts"
          data={DEMO_ACCOUNTS}
          value={selectedAccount}
          onChange={setSelectedAccount}
          clearable
          w={200}
        />
      </Group>

      <Table striped highlightOnHover withTableBorder>
        <Table.Thead>
          <Table.Tr>
            <Table.Th>Date</Table.Th>
            <Table.Th>From</Table.Th>
            <Table.Th>To</Table.Th>
            <Table.Th style={{ textAlign: 'right' }}>Amount</Table.Th>
          </Table.Tr>
        </Table.Thead>
        <Table.Tbody>
          {rows.length > 0 ? (
            rows
          ) : (
            <Table.Tr>
              <Table.Td colSpan={4}>
                <Text c="dimmed" size="sm" ta="center" py="md">
                  No transactions found for this account.
                </Text>
              </Table.Td>
            </Table.Tr>
          )}
        </Table.Tbody>
      </Table>
    </Stack>
  );
}
