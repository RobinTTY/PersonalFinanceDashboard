import { Button, Loader, Stack, Text, ThemeIcon, Title } from '@mantine/core';
import classes from './BankAuthenticationPending.module.css';

interface BankAuthenticationPendingProps {
  loading: boolean;
  disabled: boolean;
  error?: string;
  onConfirm: () => void;
}

export function BankAuthenticationPending({
  loading,
  disabled,
  error,
  onConfirm,
}: BankAuthenticationPendingProps) {
  return (
    <>
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
          {error}
        </Text>
      )}

      <Button mt="auto" loading={loading} disabled={disabled} onClick={onConfirm}>
        I&apos;ve completed the authentication
      </Button>
    </>
  );
}
