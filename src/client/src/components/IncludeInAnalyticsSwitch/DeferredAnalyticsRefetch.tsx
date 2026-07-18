import { createContext, ReactNode, useContext, useEffect, useMemo, useRef } from 'react';
import { useApolloClient } from '@apollo/client/react';
import { GetBankAccounts } from '@graphql-queries/GetBankAccounts';
import { GetNetWorthHistory } from '@graphql-queries/GetNetWorthHistory';

interface AnalyticsRefetchContextValue {
  /** Marks the analytics queries as needing a refetch on the next flush. */
  markDirty: () => void;
  /** Refetches the analytics queries if anything was marked dirty since the last flush. */
  flush: () => void;
}

const AnalyticsRefetchContext = createContext<AnalyticsRefetchContextValue | null>(null);

/**
 * Returns a callback that marks the analytics queries as needing a refetch, or `null` when used
 * outside a {@link DeferredAnalyticsRefetchProvider}. Consumers call it after mutating a value that
 * affects the analytics result sets (e.g. toggling `includeInAnalytics`).
 */
export function useMarkAnalyticsDirty() {
  return useContext(AnalyticsRefetchContext)?.markDirty ?? null;
}

/**
 * Flushes any pending analytics refetch when `opened` transitions from `true` to `false`, i.e. when
 * a surface hosting analytics toggles (such as the settings or add account modal) is closed. This
 * lets the provider live once near the app root while the refetch still fires at the right moment,
 * without churning the UI on every individual toggle.
 */
export function useFlushAnalyticsRefetchOnClose(opened: boolean) {
  const context = useContext(AnalyticsRefetchContext);
  const wasOpen = useRef(opened);

  useEffect(() => {
    if (wasOpen.current && !opened) {
      context?.flush();
    }
    wasOpen.current = opened;
  }, [opened, context]);
}

interface DeferredAnalyticsRefetchProviderProps {
  children: ReactNode;
}

/**
 * Provides deferred refetching of the analytics queries (accounts view, net worth chart) to its
 * subtree. Descendants mark the queries dirty via {@link useMarkAnalyticsDirty} when they change a
 * value the queries filter on server-side (`includeInAnalytics`), which Apollo can't re-evaluate
 * against its cache. The refetch is deferred until a surface is closed via
 * {@link useFlushAnalyticsRefetchOnClose} so that toggling doesn't churn the surrounding UI, and is
 * batched into a single refetch regardless of how many toggles happened.
 *
 * Intended to be mounted once near the app root so any page or modal can participate without
 * wrapping its own subtree.
 */
export function DeferredAnalyticsRefetchProvider({
  children,
}: DeferredAnalyticsRefetchProviderProps) {
  const client = useApolloClient();
  const dirtyRef = useRef(false);

  const value = useMemo<AnalyticsRefetchContextValue>(
    () => ({
      markDirty: () => {
        dirtyRef.current = true;
      },
      flush: () => {
        if (!dirtyRef.current) {
          return;
        }
        dirtyRef.current = false;
        void client.refetchQueries({ include: [GetBankAccounts, GetNetWorthHistory] });
      },
    }),
    [client]
  );

  return (
    <AnalyticsRefetchContext.Provider value={value}>{children}</AnalyticsRefetchContext.Provider>
  );
}
