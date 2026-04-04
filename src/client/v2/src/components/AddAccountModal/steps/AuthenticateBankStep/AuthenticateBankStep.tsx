import { useLazyQuery } from '@apollo/client/react';
import { GetAuthenticationRequest } from '@graphql-queries/GetAuthenticationRequest';
import { IconCircleCheck } from '@tabler/icons-react';
import { Button, Loader, Stack, Text, ThemeIcon, Title } from '@mantine/core';
import classes from './AuthenticateBankStep.module.css';

interface AuthenticateBankStepProps {
  authenticationId: string | undefined;
}

export function AuthenticateBankStep({ authenticationId }: AuthenticateBankStepProps) {
  const [fetchAuthenticationRequest, { loading, data, error }] = useLazyQuery(
    GetAuthenticationRequest
  );

  const handleConfirm = () => {
    if (authenticationId) {
      fetchAuthenticationRequest({ variables: { authenticationId } });
    }
  };

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

      {data && (
        <div className={classes.successMessage}>
          <ThemeIcon color="green" size="lg" radius="xl">
            <IconCircleCheck size={20} />
          </ThemeIcon>
          <Text size="sm" c="green">
            Authentication request retrieved successfully.
          </Text>
        </div>
      )}
      <Button mt="auto" loading={loading} disabled={!authenticationId} onClick={handleConfirm}>
        I&apos;ve completed the authentication
      </Button>
    </Stack>
  );
}
