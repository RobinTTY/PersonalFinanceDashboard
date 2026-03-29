import { useRef, useState } from 'react';
import { useVirtualizer } from '@tanstack/react-virtual';
import { IconSearch } from '@tabler/icons-react';
import { Loader, Stack, Text, TextInput } from '@mantine/core';
import { useQuery } from '@apollo/client/react';
import { GetBankingInstitutions } from '@graphql-queries/GetBankingInstitutions';
import { SelectionCard } from '../../SelectionCard/SelectionCard';
import classes from './SelectBankStep.module.css';

const COLUMNS = 2;
// Row height: 1px top border + 12px padding + 38px avatar (md) + 12px padding + 1px bottom border = 64px
// Gap between rows mirrors SimpleGrid spacing="sm" = 12px
const ROW_HEIGHT = 64;
const ROW_GAP = 12;

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

  const filteredBanks = banks
    .filter((bank) => bank.name.toLowerCase().includes(bankSearch.toLowerCase()))
    .sort((a, b) => a.name.localeCompare(b.name));

  const rowCount = Math.ceil(filteredBanks.length / COLUMNS);
  const parentRef = useRef<HTMLDivElement>(null);

  const rowVirtualizer = useVirtualizer({
    count: rowCount,
    getScrollElement: () => parentRef.current,
    estimateSize: () => ROW_HEIGHT + ROW_GAP,
    overscan: 5,
  });

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
      <div ref={parentRef} className={classes.bankList} style={{ flex: 1, overflowX: 'hidden' }}>
        <div style={{ height: rowVirtualizer.getTotalSize(), position: 'relative' }}>
          {rowVirtualizer.getVirtualItems().map((virtualRow) => {
            const startIndex = virtualRow.index * COLUMNS;
            const rowItems = filteredBanks.slice(startIndex, startIndex + COLUMNS);

            return (
              <div
                key={virtualRow.key}
                style={{
                  position: 'absolute',
                  top: 0,
                  left: 0,
                  width: '100%',
                  height: ROW_HEIGHT,
                  transform: `translateY(${virtualRow.start}px)`,
                  display: 'grid',
                  gridTemplateColumns: '1fr 1fr',
                  gap: 12,
                }}
              >
                {rowItems.map((bank) => {
                  const bankId = String(bank.id);
                  const logoUri = bank.logoUri ? String(bank.logoUri) : undefined;

                  return (
                    <SelectionCard
                      key={bankId}
                      id={bankId}
                      optionName={bank.name}
                      logoUri={logoUri}
                      isSelected={selectedBank === bankId}
                      onSelect={onBankSelect}
                    />
                  );
                })}
              </div>
            );
          })}
        </div>
      </div>
    </Stack>
  );
}
