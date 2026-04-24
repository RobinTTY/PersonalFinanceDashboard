import { useEffect, useRef, useState } from 'react';
import { useLazyQuery } from '@apollo/client/react';
import { GetAuthenticationRequest } from '@graphql-queries/GetAuthenticationRequest';
import { AuthenticationStatus } from '@/graphql/types/graphql';
import { IconAlertCircle, IconCircleCheck } from '@tabler/icons-react';
import { Anchor, Button, Loader, Stack, Text, ThemeIcon, Title } from '@mantine/core';
import classes from './AuthenticateBankStep.module.css';

const POLL_DELAYS = [30_000, 60_000, 120_000, 180_000, 240_000, 300_000];

type ViewState = 'waiting' | 'success' | 'failed';

interface AuthenticateBankStepProps {
  authenticationId: string | undefined;
  authenticationLink?: string;
  autoCheck?: boolean;
  onAuthenticated?: () => void;
  onClose?: () => void;
}

export function AuthenticateBankStep({
  authenticationId,
  authenticationLink,
  autoCheck = false,
  onAuthenticated,
  onClose,
}: AuthenticateBankStepProps) {
  const [viewState, setViewState] = useState<ViewState>('waiting');
  const [currentAuthLink, setCurrentAuthLink] = useState<string | undefined>(authenticationLink);

  const pollingActive = useRef(true);
  const pollIndex = useRef(0);
  const pollingTimeoutRef = useRef<ReturnType<typeof setTimeout> | undefined>(undefined);
  const isManualCheck = useRef(false);

  const [fetchAuthenticationRequest, { loading, data, error }] = useLazyQuery(
    GetAuthenticationRequest,
    { fetchPolicy: 'network-only' }
  );

  const scheduleNextPoll = () => {
    if (!pollingActive.current || !authenticationId) return;
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

  useEffect(() => {
    if (!authenticationId) return;
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
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [authenticationId]);

  useEffect(() => {
    if (!data) return;
    const status = data.authenticationRequest?.status;
    const latestLink = data.authenticationRequest?.authenticationLink;
    if (latestLink) {
      setCurrentAuthLink(String(latestLink));
    }
    if (status === AuthenticationStatus.Active) {
      pollingActive.current = false;
      clearTimeout(pollingTimeoutRef.current);
      setViewState('success');
      onAuthenticated?.();
    } else if (isManualCheck.current) {
      isManualCheck.current = false;
      pollingActive.current = false;
      setViewState('failed');
    } else {
      scheduleNextPoll();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [data]);

  const handleConfirm = () => {
    if (!authenticationId) return;
    clearTimeout(pollingTimeoutRef.current);
    isManualCheck.current = true;
    pollingActive.current = false;
    fetchAuthenticationRequest({ variables: { authenticationId } });
  };

  const handleRetry = () => {
    pollIndex.current = 0;
    isManualCheck.current = false;
    pollingActive.current = true;
    setViewState('waiting');
    scheduleNextPoll();
  };

  if (viewState === 'success') {
    return (
      <Stack className={classes.container} align="center" gap="xl">
        <div className={classes.successSection}>
          <ThemeIcon color="green" size={64} radius="xl">
            <IconCircleCheck size={36} />
          </ThemeIcon>
          <Stack gap={4} align="center">
            <Title order={5}>Authentication Successful</Title>
            <Text size="sm" c="dimmed" ta="center">
              Your bank account has been successfully linked.
            </Text>
          </Stack>
        </div>
        <Button mt="auto" onClick={onClose}>
          Done
        </Button>
      </Stack>
    );
  }

  if (viewState === 'failed') {
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

      <Button mt="auto" loading={loading} disabled={!authenticationId} onClick={handleConfirm}>
        I&apos;ve completed the authentication
      </Button>
    </Stack>
  );
}
