import { useState } from 'react';
import { IconChevronRight, IconSearch } from '@tabler/icons-react';
import { Avatar, Group, SimpleGrid, Stack, Text, TextInput, UnstyledButton } from '@mantine/core';
import { useQuery } from '@apollo/client/react';
import { GetBankingInstitutions } from '@graphql-queries/GetBankingInstitutions';
import classes from '../AddAccountModal.module.css';

interface Bank {
  id: string;
  name: string;
  initials: string;
  color: string;
}

const dummyBanks: Bank[] = [
  { id: '1', name: 'Kreissparkasse Tübingen', initials: 'KT', color: 'red' },
  { id: '2', name: 'Sparkasse Bad Neustadt', initials: 'SN', color: 'red' },
  { id: '3', name: 'Kreissparkasse Rhein-Neckar', initials: 'KR', color: 'red' },
  { id: '4', name: 'Bank für Kirche und Diakonie', initials: 'BK', color: 'teal' },
  { id: '5', name: 'Hagnauer Volksbank', initials: 'HV', color: 'blue' },
  { id: '6', name: 'Gabler-Saliter Bankhaus', initials: 'GS', color: 'yellow' },
  { id: '7', name: 'Deutsche Bank', initials: 'DB', color: 'blue' },
  { id: '8', name: 'Commerzbank', initials: 'CB', color: 'yellow' },
  { id: '9', name: 'ING-DiBa', initials: 'IN', color: 'orange' },
  { id: '10', name: 'DKB Deutsche Kreditbank', initials: 'DK', color: 'cyan' },
  { id: '11', name: 'Comdirect', initials: 'CD', color: 'yellow' },
  { id: '12', name: 'N26', initials: 'N2', color: 'dark' },
];

interface SelectBankStepProps {
  selectedBank: string | undefined;
  onBankSelect: (bankId: string) => void;
}

export function SelectBankStep({ selectedBank, onBankSelect }: SelectBankStepProps) {
  const [bankSearch, setBankSearch] = useState('');
  const { loading, error, data } = useQuery(GetBankingInstitutions, {
    variables: { first: 3000 },
  });

  const filteredBanks = dummyBanks.filter((bank) =>
    bank.name.toLowerCase().includes(bankSearch.toLowerCase()),
  );

  return (
    <Stack gap="md" style={{ height: '100%' }}>
      <TextInput
        placeholder="Filter banks..."
        leftSection={<IconSearch size={16} />}
        value={bankSearch}
        onChange={(e) => setBankSearch(e.currentTarget.value)}
      />
      <SimpleGrid cols={2} spacing="sm">
        {filteredBanks.map((bank) => (
          <UnstyledButton
            key={bank.id}
            className={classes.bankCard}
            data-selected={selectedBank === bank.id || undefined}
            onClick={() => onBankSelect(bank.id)}
          >
            <Group justify="space-between" wrap="nowrap">
              <Group wrap="nowrap" gap="sm" style={{ minWidth: 0 }}>
                <Avatar color={bank.color} radius="sm" size="md" flex="0 0 auto">
                  {bank.initials}
                </Avatar>
                <Text fw={500} size="sm" truncate>
                  {bank.name}
                </Text>
              </Group>
              <IconChevronRight size={16} style={{ flexShrink: 0 }} />
            </Group>
          </UnstyledButton>
        ))}
      </SimpleGrid>
    </Stack>
  );
}
