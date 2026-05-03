import { useEffect, useRef, useState } from 'react';
import { useLazyQuery } from '@apollo/client/react';
import { GetAuthRequestWithAccounts } from '@graphql-queries/GetAuthRequestAndAccounts';
import { Stack } from '@mantine/core';
import { AuthenticationStatus } from '@/graphql/types/graphql';
import { BankAuthenticationFailed } from './sub-components/BankAuthenticationFailed/BankAuthenticationFailed';
import { BankAuthenticationPending } from './sub-components/BankAuthenticationPending/BankAuthenticationPending';
import { BankAuthenticationSuccess } from './sub-components/BankAuthenticationSuccess/BankAuthenticationSuccess';
import classes from './AuthenticateBankStep.module.css';

const POLL_DELAYS = [30_000, 60_000, 120_000, 180_000, 240_000, 300_000];

enum AuthenticationState {
  Waiting,
  Success,
  Failed,
}

interface AuthenticateBankStepProps {
  authenticationId: string | undefined;
  authenticationLink?: string;
  autoCheck?: boolean;
  onAuthenticated?: () => void;
}

/**
 * Step that guides the user through external bank authentication via GoCardless.
 *
 * Polls the authentication request at increasing intervals to detect completion.
 * Renders one of three states: waiting, success, or failed.
 *
 * @param authenticationId - ID of the authentication request to poll.
 * @param authenticationLink - Initial GoCardless authentication URL shown to the user.
 * @param autoCheck - When true, performs an immediate check instead of waiting for the first poll interval.
 * @param onAuthenticated - Called when the authentication request transitions to Active.
 */
export function AuthenticateBankStep({
  authenticationId,
  authenticationLink,
  autoCheck = false,
  onAuthenticated,
}: AuthenticateBankStepProps) {
  const [authState, setAuthState] = useState<AuthenticationState>(AuthenticationState.Waiting);
  const [currentAuthLink, setCurrentAuthLink] = useState<string | undefined>(authenticationLink);

  const pollingActive = useRef(true);
  const pollIndex = useRef(0);
  const pollingTimeoutRef = useRef<ReturnType<typeof setTimeout> | undefined>(undefined);
  const userInitiatedAuthFetch = useRef(false);

  const [fetchAuthenticationRequest, { loading, data, error }] = useLazyQuery(
    GetAuthRequestWithAccounts,
    { fetchPolicy: 'network-only' }
  );

  /** Schedules the next authentication status poll after the delay defined by the current poll index, stopping polling once all delays are exhausted. */
  const scheduleNextPoll = () => {
    if (!pollingActive.current || !authenticationId) {
      return;
    }

    if (pollIndex.current >= POLL_DELAYS.length) {
      pollingActive.current = false;
      return;
    }

    const delay = POLL_DELAYS[pollIndex.current];
    pollIndex.current += 1;

    pollingTimeoutRef.current = setTimeout(() => {
      fetchAuthenticationRequest({ variables: { authenticationId } });
    }, delay);
  };

  /** Starts polling when an authenticationId becomes available, performing an immediate check if autoCheck is set, and cleans up any pending poll on unmount. */
  useEffect(() => {
    if (!authenticationId) {
      return;
    }

    pollingActive.current = true;
    if (autoCheck) {
      fetchAuthenticationRequest({ variables: { authenticationId } });
    } else {
      scheduleNextPoll();
    }

    return () => {
      pollingActive.current = false;
      clearTimeout(pollingTimeoutRef.current);
    };
  }, [authenticationId]);

  /** Reacts to a completed authentication query by transitioning to success, failed, or scheduling the next poll depending on the returned status. */
  useEffect(() => {
    if (!data) {
      return;
    }

    const status = data.authenticationRequest?.status;
    const latestLink = data.authenticationRequest?.authenticationLink;

    if (latestLink) {
      setCurrentAuthLink(String(latestLink));
    }

    if (status === AuthenticationStatus.Active) {
      pollingActive.current = false;
      clearTimeout(pollingTimeoutRef.current);
      setAuthState(AuthenticationState.Success);
      onAuthenticated?.();
    } else if (userInitiatedAuthFetch.current) {
      userInitiatedAuthFetch.current = false;
      pollingActive.current = false;
      setAuthState(AuthenticationState.Failed);
    } else {
      scheduleNextPoll();
    }
  }, [data]);

  /** Handles the user clicking the confirm button by cancelling any pending poll and immediately querying the authentication status. */
  const handleAuthCompletedButtonPress = () => {
    if (!authenticationId) {
      return;
    }

    clearTimeout(pollingTimeoutRef.current);
    userInitiatedAuthFetch.current = true;
    pollingActive.current = false;
    fetchAuthenticationRequest({ variables: { authenticationId } });
  };

  /** Resets the polling state back to the beginning and returns the component to the waiting view so the user can try again. */
  const handleRetry = () => {
    pollIndex.current = 0;
    userInitiatedAuthFetch.current = false;
    pollingActive.current = true;
    setAuthState(AuthenticationState.Waiting);
    scheduleNextPoll();
  };

  if (authState === AuthenticationState.Success) {
    const associatedAccounts = data?.authenticationRequest?.associatedAccounts ?? [];
    return <BankAuthenticationSuccess accounts={associatedAccounts} />;
  }

  if (authState === AuthenticationState.Failed) {
    return (
      <BankAuthenticationFailed
        loading={loading}
        authLink={currentAuthLink}
        onRetry={handleRetry}
      />
    );
  }

  return (
    <Stack className={classes.container} align="center" gap="xl">
      <BankAuthenticationPending
        loading={loading}
        disabled={!authenticationId}
        error={error?.message}
        onConfirm={handleAuthCompletedButtonPress}
      />
    </Stack>
  );
}
