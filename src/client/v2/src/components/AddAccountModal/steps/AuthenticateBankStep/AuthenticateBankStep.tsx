import { useEffect, useRef, useState } from 'react';
import { useLazyQuery } from '@apollo/client/react';
import { GetAuthRequestWithAccounts } from '@graphql-queries/GetAuthRequestAndAccounts';
import { IconAlertCircle, IconCircleCheck } from '@tabler/icons-react';
import {
  Anchor,
  Avatar,
  Badge,
  Button,
  Divider,
  Group,
  Loader,
  Stack,
  Text,
  ThemeIcon,
  Title,
} from '@mantine/core';
import { AuthenticationStatus } from '@/graphql/types/graphql';
import { getInitials } from '@/utility/getInitials';
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
 * @param onClose - Called when the user dismisses the success view.
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
    const institution = associatedAccounts[0]?.associatedInstitution;
    const accountCount = associatedAccounts.length;

    return (
      <Stack className={classes.container} align="center" gap="lg">
        <Group gap="sm" align="center">
          <ThemeIcon color="green" size={40} radius="xl">
            <IconCircleCheck size={22} />
          </ThemeIcon>
          <Stack gap={2}>
            <Title order={5}>Authentication Successful</Title>
            <Text size="sm" c="dimmed">
              {accountCount === 1
                ? 'Your bank account has been successfully linked.'
                : accountCount > 1
                  ? `${accountCount} bank accounts have been successfully linked.`
                  : 'Your bank has been successfully linked.'}
            </Text>
          </Stack>
        </Group>

        {institution && (
          <Group className={classes.bankHeader} gap="md" wrap="nowrap">
            <Avatar
              src={institution.logoUri ? String(institution.logoUri) : undefined}
              radius="sm"
              size="lg"
              flex="0 0 auto"
            >
              {getInitials(institution.name)}
            </Avatar>
            <Stack gap={2} style={{ minWidth: 0 }}>
              <Text fw={600} size="md" truncate>
                {institution.name}
              </Text>
              <Text size="sm" c="dimmed">
                {institution.bic}
              </Text>
            </Stack>
          </Group>
        )}

        {associatedAccounts.length > 0 && (
          <Stack gap="sm" w="100%">
            {associatedAccounts.map((account) => {
              const hasNameOrDescription = account.name || account.description;
              const balanceValue =
                account.balance != null
                  ? account.currency
                    ? `${String(account.balance)} ${account.currency}`
                    : String(account.balance)
                  : null;

              return (
                <div key={String(account.id)} className={classes.accountCard}>
                  {hasNameOrDescription && (
                    <Stack gap={2} mb="md">
                      {account.name && (
                        <Text fw={600} size="md">
                          {account.name}
                        </Text>
                      )}
                      {account.description && account.description !== account.name && (
                        <Text size="sm" c="dimmed">
                          {account.description}
                        </Text>
                      )}
                    </Stack>
                  )}

                  <Stack gap="xs">
                    {account.accountType && (
                      <Group justify="space-between" wrap="nowrap">
                        <Text size="sm" c="dimmed">
                          Type
                        </Text>
                        <Badge variant="light" size="sm">
                          {account.accountType}
                        </Badge>
                      </Group>
                    )}
                    {hasNameOrDescription && account.accountType && <Divider />}
                    {balanceValue && (
                      <Group justify="space-between" wrap="nowrap">
                        <Text size="sm" c="dimmed">
                          Balance
                        </Text>
                        <Text size="sm" fw={600}>
                          {balanceValue}
                        </Text>
                      </Group>
                    )}
                    {account.ownerName && (
                      <Group justify="space-between" wrap="nowrap">
                        <Text size="sm" c="dimmed">
                          Owner
                        </Text>
                        <Text size="sm" fw={500}>
                          {account.ownerName}
                        </Text>
                      </Group>
                    )}
                  </Stack>
                </div>
              );
            })}
          </Stack>
        )}
      </Stack>
    );
  }

  if (authState === AuthenticationState.Failed) {
    return (
      <Stack className={classes.container} align="center" gap="xl">
        <div className={classes.failureSection}>
          <ThemeIcon color="orange" size={64} radius="xl">
            <IconAlertCircle size={36} />
          </ThemeIcon>
          <Stack gap={4} align="center">
            <Title order={5}>Authentication Not Completed</Title>
            <Text size="sm" c="dimmed" ta="center">
              Your authentication does not appear to be complete yet. If you haven&apos;t finished
              the external authentication, please follow the link below.
            </Text>
          </Stack>
        </div>
        {currentAuthLink && (
          <Anchor href={currentAuthLink} target="_blank" rel="noopener noreferrer" size="sm">
            Open GoCardless authentication
          </Anchor>
        )}
        <Button mt="auto" loading={loading} onClick={handleRetry}>
          Check again
        </Button>
      </Stack>
    );
  }

  return (
    <Stack className={classes.container} align="center" gap="xl">
      <div className={classes.loaderSection}>
        <Loader size="lg" />
        <Stack gap={4} align="center">
          <Title order={5}>Waiting for Authentication</Title>
          <Text size="sm" c="dimmed" ta="center">
            A GoCardless authentication page has been opened. Please authenticate with your bank
            there and return here once you have completed the process.
          </Text>
        </Stack>
      </div>

      <div className={classes.instructionsSection}>
        <Stack gap="sm">
          <div className={classes.instructionItem}>
            <ThemeIcon size="sm" variant="light" radius="xl">
              <Text size="xs" fw={700}>
                1
              </Text>
            </ThemeIcon>
            <Text size="sm">
              Open the GoCardless authentication link and log in with your bank credentials.
            </Text>
          </div>
          <div className={classes.instructionItem}>
            <ThemeIcon size="sm" variant="light" radius="xl">
              <Text size="xs" fw={700}>
                2
              </Text>
            </ThemeIcon>
            <Text size="sm">Grant GoCardless permission to access your account data.</Text>
          </div>
          <div className={classes.instructionItem}>
            <ThemeIcon size="sm" variant="light" radius="xl">
              <Text size="xs" fw={700}>
                3
              </Text>
            </ThemeIcon>
            <Text size="sm">Return to this page and click the confirm button below.</Text>
          </div>
        </Stack>
      </div>

      {error && (
        <Text c="red" size="sm" ta="center">
          {error.message}
        </Text>
      )}

      <Button
        mt="auto"
        loading={loading}
        disabled={!authenticationId}
        onClick={handleAuthCompletedButtonPress}
      >
        I&apos;ve completed the authentication
      </Button>
    </Stack>
  );
}
