import { useEffect, useMemo, useRef, useState } from 'react';
import { useQuery } from '@apollo/client/react';
import { IconAlertCircle } from '@tabler/icons-react';
import { useVirtualizer } from '@tanstack/react-virtual';
import { Alert, Box, Group, Loader, Select, Stack, Text, Title } from '@mantine/core';
import { GetAuthRequestsWithAccounts } from '@graphql-queries/GetAuthRequestsWithAccounts';
import { GetTransactionsByAccountId } from '@graphql-queries/GetTransactionsByAccountId';

const PAGE_SIZE = 50;
const ROW_HEIGHT = 52;
const VIRTUAL_OVERSCAN = 8;
const COLUMN_TEMPLATE = '140px minmax(0, 1fr) minmax(0, 1fr) 160px';

function formatDate(date: Date | null | undefined): string {
  if (!date) {
    return '—';
  }
  return date.toLocaleDateString('en-US', {
    day: '2-digit',
    month: 'short',
    year: 'numeric',
  });
}

function formatAmount(amount: number, currency: string): string {
  const sign = amount >= 0 ? '+' : '−';
  return `${sign}${Math.abs(amount).toFixed(2)} ${currency}`;
}

export function TransactionsPage() {
  const { data: accountsData, loading: loadingAccounts } = useQuery(GetAuthRequestsWithAccounts);

  const accounts = useMemo(() => {
    const all =
      accountsData?.authenticationRequests.flatMap((req) =>
        req.associatedAccounts
          .filter((acc) => acc.id != null)
          .map((acc) => ({
            value: String(acc.id),
            label: acc.name ?? acc.iban ?? 'Unnamed account',
          }))
      ) ?? [];
    return Array.from(new Map(all.map((a) => [a.value, a])).values());
  }, [accountsData]);

  const [accountId, setAccountId] = useState<string | null>(null);

  useEffect(() => {
    if (!accountId && accounts.length > 0) {
      setAccountId(accounts[0].value);
    }
  }, [accounts, accountId]);

  const { data, loading, error, fetchMore } = useQuery(GetTransactionsByAccountId, {
    variables: { accountId: accountId ?? '', first: PAGE_SIZE },
    skip: !accountId,
    notifyOnNetworkStatusChange: true,
  });

  const edges = data?.transactionsByAccountId?.edges ?? [];
  const pageInfo = data?.transactionsByAccountId?.pageInfo;

  const parentRef = useRef<HTMLDivElement>(null);
  const fetchingMoreRef = useRef(false);

  const virtualizer = useVirtualizer({
    count: edges.length,
    getScrollElement: () => parentRef.current,
    estimateSize: () => ROW_HEIGHT,
    overscan: VIRTUAL_OVERSCAN,
  });

  const virtualItems = virtualizer.getVirtualItems();

  useEffect(() => {
    parentRef.current?.scrollTo({ top: 0 });
  }, [accountId]);

  useEffect(() => {
    const last = virtualItems[virtualItems.length - 1];
    if (!last || !pageInfo?.hasNextPage || fetchingMoreRef.current) {
      return;
    }
    if (last.index < edges.length - VIRTUAL_OVERSCAN) {
      return;
    }

    fetchingMoreRef.current = true;
    fetchMore({
      variables: {
        accountId,
        first: PAGE_SIZE,
        after: pageInfo.endCursor,
      },
    }).finally(() => {
      fetchingMoreRef.current = false;
    });
  }, [virtualItems, edges.length, pageInfo, fetchMore, accountId]);

  const showInitialLoader = !!accountId && loading && edges.length === 0;
  const showEmptyState = !!accountId && !loading && edges.length === 0;
  const showNoAccounts = !loadingAccounts && accounts.length === 0;

  return (
    <Stack style={{ height: '100%' }}>
      <Group justify="space-between" align="flex-end">
        <Title order={2} m={0}>
          Transactions
        </Title>
        <Select
          placeholder={loadingAccounts ? 'Loading accounts…' : 'Select an account'}
          data={accounts}
          value={accountId}
          onChange={setAccountId}
          disabled={loadingAccounts || accounts.length === 0}
          searchable
          w={260}
        />
      </Group>

      {error ? (
        <Alert color="red" icon={<IconAlertCircle size={16} />} title="Failed to load transactions">
          {error.message}
        </Alert>
      ) : null}

      <Box
        style={{
          display: 'grid',
          gridTemplateColumns: COLUMN_TEMPLATE,
          gap: 'var(--mantine-spacing-md)',
          padding: 'var(--mantine-spacing-sm) var(--mantine-spacing-md)',
          borderBottom:
            '1px solid light-dark(var(--mantine-color-gray-3), var(--mantine-color-dark-4))',
        }}
      >
        <Text size="xs" fw={600} c="dimmed" tt="uppercase">
          Date
        </Text>
        <Text size="xs" fw={600} c="dimmed" tt="uppercase">
          From
        </Text>
        <Text size="xs" fw={600} c="dimmed" tt="uppercase">
          To
        </Text>
        <Text size="xs" fw={600} c="dimmed" tt="uppercase" ta="right">
          Amount
        </Text>
      </Box>

      {showNoAccounts ? (
        <Stack align="center" justify="center" style={{ flex: 1, minHeight: 0 }}>
          <Text size="sm" c="dimmed">
            Connect a bank account to view transactions.
          </Text>
        </Stack>
      ) : showInitialLoader ? (
        <Stack align="center" justify="center" style={{ flex: 1, minHeight: 0 }}>
          <Loader size="sm" />
        </Stack>
      ) : showEmptyState ? (
        <Stack align="center" justify="center" style={{ flex: 1, minHeight: 0 }}>
          <Text size="sm" c="dimmed">
            No transactions for this account.
          </Text>
        </Stack>
      ) : (
        <Box
          ref={parentRef}
          style={{
            flex: 1,
            minHeight: 0,
            overflow: 'auto',
            contain: 'strict',
          }}
        >
          <div
            style={{
              height: virtualizer.getTotalSize(),
              position: 'relative',
              width: '100%',
            }}
          >
            {virtualItems.map((virtualRow) => {
              const transaction = edges[virtualRow.index].node;
              return (
                <div
                  key={virtualRow.key}
                  style={{
                    position: 'absolute',
                    top: 0,
                    left: 0,
                    width: '100%',
                    transform: `translateY(${virtualRow.start}px)`,
                    height: ROW_HEIGHT,
                    display: 'grid',
                    gridTemplateColumns: COLUMN_TEMPLATE,
                    gap: 'var(--mantine-spacing-md)',
                    alignItems: 'center',
                    padding: '0 var(--mantine-spacing-md)',
                    borderBottom:
                      '1px solid light-dark(var(--mantine-color-gray-2), var(--mantine-color-dark-5))',
                  }}
                >
                  <Text size="sm">{formatDate(transaction.valueDate)}</Text>
                  <Text size="sm" truncate>
                    {transaction.payer ?? '—'}
                  </Text>
                  <Text size="sm" truncate>
                    {transaction.payee ?? '—'}
                  </Text>
                  <Text
                    size="sm"
                    fw={600}
                    ta="right"
                    c={transaction.amount >= 0 ? 'green' : 'red'}
                  >
                    {formatAmount(transaction.amount, transaction.currency)}
                  </Text>
                </div>
              );
            })}
          </div>
        </Box>
      )}
    </Stack>
  );
}
