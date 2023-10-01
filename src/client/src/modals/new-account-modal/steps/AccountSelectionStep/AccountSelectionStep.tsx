import { useQuery } from '@apollo/client';
import { Box, Center, Loader, SimpleGrid, Text, Group, Badge, Button } from '@mantine/core';
import { useForm } from '@mantine/form';

import { GetAccountsQuery } from '@graphql-queries/GetAccounts';
import { ContentCheckbox } from '@components/image-checkbox/ContentCheckbox';
import { AccountSelection, AccountSelectionStepProps } from './AccountSelectionStepProps';

export const AccountSelectionStep = ({
  authentication,
  onAccountsSelected,
}: AccountSelectionStepProps) => {
  // TODO: duplicate code (AccountSummary)
  const formatter = new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',

    // These options are needed to round to whole numbers if that's what you want.
    //minimumFractionDigits: 0, // (this suffices for whole numbers, but will print 2500.10 as $2,500.1)
    //maximumFractionDigits: 0, // (causes 2500.99 to be printed as $2,501)
  });

  const { loading, data } = useQuery(GetAccountsQuery, {
    variables: {
      accountIds: authentication.associatedAccounts,
    },
  });

  const form = useForm({
    initialValues: {
      accounts: [] as AccountSelection[],
    },
  });

  const accounts = data?.accounts?.edges?.map(
    (account) =>
      ({
        id: account.node.id,
        name: account.node.name,
        balance: account.node.balance,
        iban: account.node.iban,
        checked: false,
      } as AccountSelection)
  );

  // TODO: Create reusable loader component
  if (loading) {
    return (
      <Center h="100%">
        <Loader color="violet" />
      </Center>
    );
  }

  const formValues = { accounts: accounts ?? [] };
  if (form.values.accounts.length === 0) {
    form.setInitialValues(formValues);
    form.setValues(formValues);
  }

  // TODO: should be light background when selected, dark when not
  // TODO: Can minimum width of grid be set to size of 2 child elements?
  // TODO: Implement simple selection if only one account is available (no selection necessary, just yes/no)
  return (
    <form onSubmit={form.onSubmit((values) => onAccountsSelected(values.accounts))}>
      <SimpleGrid cols={{ base: 1, sm: data?.accounts?.edges?.length === 1 ? 1 : 2 }}>
        {form.values.accounts.map((account, index) => (
          <Box key={account.id} pt="xs">
            <ContentCheckbox
              checkboxProps={form.getInputProps(`accounts.${index}.checked`, {
                type: 'checkbox',
              })}
              onChange={() => form.setFieldValue(`accounts.${index}.checked`, !account.checked)}
            >
              <Group>
                <Box maw={150}>
                  <Text fw={500} size="md" truncate="end">
                    {account.name}
                  </Text>
                </Box>
                <Badge size="md" variant="light">
                  {formatter.format(account.balance)}
                </Badge>
              </Group>
              <Text c="dimmed" size="xs">
                {account.iban}
              </Text>
            </ContentCheckbox>
          </Box>
        ))}
      </SimpleGrid>
      <Center pt="lg">
        <Button size="md" type="submit">
          Import Accounts
        </Button>
      </Center>
    </form>
  );
};
