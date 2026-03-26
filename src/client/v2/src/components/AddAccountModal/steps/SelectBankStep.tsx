import { useState } from 'react';
import { IconChevronRight, IconSearch } from '@tabler/icons-react';
import { Avatar, Group, Loader, SimpleGrid, Stack, Text, TextInput, UnstyledButton } from '@mantine/core';
import { useQuery } from '@apollo/client/react';
import { GetBankingInstitutions } from '@graphql-queries/GetBankingInstitutions';
import classes from '../AddAccountModal.module.css';

interface SelectBankStepProps {
  selectedBank: string | undefined;
  onBankSelect: (bankId: string) => void;
}

export function SelectBankStep({ selectedBank, onBankSelect }: SelectBankStepProps) {
  const [bankSearch, setBankSearch] = useState('');
  const { loading, error, data } = useQuery(GetBankingInstitutions, {
    variables: { first: 3000 },
  });

  const banks = data?.bankingInstitutions?.edges?.map((edge) => edge.node) ?? [];

  const filteredBanks = banks.filter((bank) =>
    bank.name.toLowerCase().includes(bankSearch.toLowerCase()),
  );

  return (
    <Stack gap="md" style={{ height: '100%', overflow: 'hidden' }}>
      <TextInput
        placeholder="Filter banks..."
        leftSection={<IconSearch size={16} />}
        value={bankSearch}
        onChange={(e) => setBankSearch(e.currentTarget.value)}
        style={{ flexShrink: 0 }}
      />
      {loading && <Loader mx="auto" />}
      {error && <Text c="red" size="sm">{error.message}</Text>}
      <SimpleGrid cols={2} spacing="sm" className={classes.bankList}>
        {filteredBanks.map((bank) => {
          const bankId = String(bank.id);
          const logoUri = bank.logoUri ? String(bank.logoUri) : undefined;

          return (
            <UnstyledButton
              key={bankId}
              className={classes.bankCard}
              data-selected={selectedBank === bankId || undefined}
              onClick={() => onBankSelect(bankId)}
            >
              <Group justify="space-between" wrap="nowrap">
                <Group wrap="nowrap" gap="sm" style={{ minWidth: 0 }}>
                  <Avatar src={logoUri} radius="sm" size="md" flex="0 0 auto">
                    {getInitials(bank.name)}
                  </Avatar>
                  <Text fw={500} size="sm" truncate>
                    {bank.name}
                  </Text>
                </Group>
                <IconChevronRight size={16} style={{ flexShrink: 0 }} />
              </Group>
            </UnstyledButton>
          );
        })}
      </SimpleGrid>
    </Stack>
  );
}

/**
 * Generates initials from a bank name for use in the Avatar component when no logo is available.
 * @param name The name of the bank
 * @returns A string of initials (up to 2 characters) derived from the bank name
 */
function getInitials(name: string): string {
  return name
    .split(' ')
    .slice(0, 2)
    .map((word) => word[0])
    .join('')
    .toUpperCase();
}
