import { useEffect, useMemo, useRef, useState } from 'react';
import { useQuery } from '@apollo/client/react';
import { IconAlertCircle } from '@tabler/icons-react';
import { useVirtualizer } from '@tanstack/react-virtual';
import {
  Alert,
  Box,
  Loader,
  Stack,
  Text,
} from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { AccountsHeader } from '@/components/AccountsHeader/AccountsHeader';
import { AddAccountModal, PendingAuthState } from '@/components/AddAccountModal/AddAccountModal';
import { GetAuthRequestsWithAccounts } from '@graphql-queries/GetAuthRequestsWithAccounts';
import { GetTransactionsByAccountId } from '@graphql-queries/GetTransactionsByAccountId';
import { formatAmount, formatDate } from '@utility';
import classes from './Accounts.page.module.css';

const PAGE_SIZE = 50;
const ROW_HEIGHT = 52;
const VIRTUAL_OVERSCAN = 8;

export function AccountsPage() {
  const [addAccountOpened, { open: openAddAccount, close: closeAddAccount }] = useDisclosure(false);
  const [pendingAuth, setPendingAuth] = useState<PendingAuthState | undefined>(undefined);

  useEffect(() => {
    const stored = localStorage.getItem('pendingGoCardlessAuth');
    if (stored) {
      localStorage.removeItem('pendingGoCardlessAuth');
      setPendingAuth(JSON.parse(stored) as PendingAuthState);
      openAddAccount();
    }
  }, [openAddAccount]);

  const handleModalClose = () => {
    setPendingAuth(undefined);
    closeAddAccount();
  };

  const { data: accountsData, loading: loadingAccounts } = useQuery(GetAuthRequestsWithAccounts);

  const accounts = useMemo(() => {
    const all =
      accountsData?.authenticationRequests.flatMap((req) =>
        req.associatedAccounts.filter((acc) => acc.id != null)
      ) ?? [];
    return Array.from(new Map(all.map((a) => [String(a.id), a])).values());
  }, [accountsData]);

  const [accountId, setAccountId] = useState<string | null>(null);

  // Auto-select the first account once the accounts query resolves, so the page
  // shows transactions immediately instead of an empty selector.
  useEffect(() => {
    if (!accountId && accounts.length > 0) {
      setAccountId(String(accounts[0].id));
    }
  }, [accounts, accountId]);

  const currentAccount = useMemo(
    () => accounts.find((a) => String(a.id) === accountId) ?? null,
    [accounts, accountId]
  );

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

  // Reset scroll to the top when switching accounts. Each accountId has its own
  // cached list (see field policy in App.tsx), so the previous scroll offset
  // would otherwise be meaningless against a different list.
  useEffect(() => {
    parentRef.current?.scrollTo({ top: 0 });
  }, [accountId]);

  // Infinite-scroll loader: when the virtualizer renders within VIRTUAL_OVERSCAN
  // rows of the end and the server reports more pages, request the next page via
  // the cursor. fetchingMoreRef guards against the effect firing repeatedly while
  // a fetch is already in flight (each scroll tick re-runs this effect).
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
    <Stack className={classes.pageStack}>
      <AccountsHeader
        currentAccount={currentAccount}
        accounts={accounts}
        loadingAccounts={loadingAccounts}
        onAccountChange={setAccountId}
        onAddAccount={openAddAccount}
      />

      <AddAccountModal
        opened={addAccountOpened}
        onClose={handleModalClose}
        pendingAuth={pendingAuth}
      />

      {error ? (
        <Alert color="red" icon={<IconAlertCircle size={16} />} title="Failed to load transactions">
          {error.message}
        </Alert>
      ) : null}

      <Box className={classes.columnHeader}>
        <Text size="xs" fw={600} c="dimmed" tt="uppercase">
          Date
        </Text>
        <Text size="xs" fw={600} c="dimmed" tt="uppercase">
          Description
        </Text>
        <Text size="xs" fw={600} c="dimmed" tt="uppercase" ta="right">
          Amount
        </Text>
      </Box>

      {showNoAccounts ? (
        <Stack align="center" justify="center" className={classes.centeredFlex}>
          <Text size="sm" c="dimmed">
            Connect a bank account to view transactions.
          </Text>
        </Stack>
      ) : showInitialLoader ? (
        <Stack align="center" justify="center" className={classes.centeredFlex}>
          <Loader size="sm" />
        </Stack>
      ) : showEmptyState ? (
        <Stack align="center" justify="center" className={classes.centeredFlex}>
          <Text size="sm" c="dimmed">
            No transactions for this account.
          </Text>
        </Stack>
      ) : (
        <Box ref={parentRef} className={classes.scrollContainer}>
          <div className={classes.virtualList} style={{ height: virtualizer.getTotalSize() }}>
            {virtualItems.map((virtualRow) => {
              const transaction = edges[virtualRow.index].node;
              return (
                <div
                  key={virtualRow.key}
                  className={classes.virtualRow}
                  style={{ transform: `translateY(${virtualRow.start}px)` }}
                >
                  <Text size="sm">{formatDate(transaction.valueDate)}</Text>
                  <Text size="sm" truncate>
                    {transaction.amount < 0 ? transaction.payer ?? '—' : transaction.payee ?? '—'}
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
