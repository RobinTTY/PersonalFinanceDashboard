import { Anchor, Button, Stack, Text, ThemeIcon, Title } from '@mantine/core';
import { IconAlertCircle } from '@tabler/icons-react';
import classes from './BankAuthenticationFailed.module.css';

interface BankAuthenticationFailedProps {
  loading: boolean;
  authLink?: string;
  onRetry: () => void;
}

export function BankAuthenticationFailed({
  loading,
  authLink,
  onRetry,
}: BankAuthenticationFailedProps) {
  return (
    <Stack h="100%" align="center" gap="xl">
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
      {authLink && (
        <Anchor href={authLink} target="_blank" rel="noopener noreferrer" size="sm">
          Open GoCardless authentication
        </Anchor>
      )}
      <Button mt="auto" loading={loading} onClick={onRetry}>
        Check again
      </Button>
    </Stack>
  );
}
