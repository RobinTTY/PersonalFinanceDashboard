import { IconAlertTriangle } from '@tabler/icons-react';
import { isRouteErrorResponse, useRouteError } from 'react-router-dom';
import { Button, Code, Stack, Text, ThemeIcon, Title } from '@mantine/core';

function getErrorDetails(error: unknown): { title: string; message: string } {
  if (isRouteErrorResponse(error)) {
    return error.status === 404
      ? { title: 'Page not found', message: "The page you're looking for doesn't exist." }
      : { title: `${error.status} ${error.statusText}`, message: String(error.data ?? '') };
  }

  return {
    title: 'Something went wrong',
    message: 'An unexpected error occurred while displaying this page.',
  };
}

/**
 * Rendered by the router instead of a route that failed to render, so an error doesn't blank the app.
 */
export function RouteErrorBoundary() {
  const error = useRouteError();
  const { title, message } = getErrorDetails(error);

  return (
    <Stack align="center" justify="center" gap="md" mih="60vh" p="md" ta="center">
      <ThemeIcon color="red" variant="light" size={56} radius="xl">
        <IconAlertTriangle size={32} />
      </ThemeIcon>
      <Title order={2}>{title}</Title>
      <Text c="dimmed">{message}</Text>
      {import.meta.env.DEV && error instanceof Error && (
        <Code block maw={800} ta="left">
          {error.stack ?? error.message}
        </Code>
      )}
      <Button onClick={() => window.location.reload()}>Reload page</Button>
    </Stack>
  );
}
