import { useState } from 'react';
import { Group, Stack, Text } from '@mantine/core';
import { CountryFlag } from '../CountryFlag/CountryFlag';
import { ComponentPreview } from '../storybook/ComponentPreview';
import { SelectionCard } from './SelectionCard';
import barclaysLogo from './story-resources/barclays-logo.jpg';
import revolutLogo from './story-resources/revolut-logo.jpg';

export default { title: 'SelectionCard' };

export function Usage() {
  const [selectedId, setSelectedId] = useState<string | null>(null);

  return (
    <ComponentPreview canvas={{ center: true, maxWidth: 320 }} withSpacing>
      <Stack gap="xs">
        <SelectionCard
          id="savings"
          optionName="Savings Account"
          isSelected={selectedId === 'savings'}
          onSelect={setSelectedId}
        />
        <SelectionCard
          id="investment"
          optionName="Investment Account"
          isSelected={selectedId === 'investment'}
          onSelect={setSelectedId}
        />
      </Stack>
    </ComponentPreview>
  );
}

export function Selected() {
  return (
    <ComponentPreview canvas={{ center: true, maxWidth: 320 }} withSpacing>
      <SelectionCard id="savings" optionName="Savings Account" isSelected onSelect={() => {}} />
    </ComponentPreview>
  );
}

export function Unselected() {
  return (
    <ComponentPreview canvas={{ center: true, maxWidth: 320 }} withSpacing>
      <SelectionCard
        id="savings"
        optionName="Savings Account"
        isSelected={false}
        onSelect={() => {}}
      />
    </ComponentPreview>
  );
}

export function WithSubtitle() {
  const [selectedId, setSelectedId] = useState<string | null>(null);

  const banks = [
    { id: 'ikano-se', bic: 'IKANOSES1', countries: ['SE'] },
    { id: 'ikano-de', bic: 'IKANODE1', countries: ['DE'] },
    { id: 'ikano-multi', bic: 'IKANOFIHH', countries: ['FI', 'NO', 'DK', 'AT'] },
  ];

  return (
    <ComponentPreview canvas={{ center: true, maxWidth: 320 }} withSpacing>
      <Stack gap="xs">
        {banks.map((bank) => (
          <SelectionCard
            key={bank.id}
            id={bank.id}
            optionName="Ikano Bank"
            isSelected={selectedId === bank.id}
            onSelect={setSelectedId}
            subtitle={
              <Group gap={6} wrap="nowrap" style={{ minWidth: 0 }}>
                {bank.countries.slice(0, 3).map((country) => (
                  <CountryFlag key={country} code={country} />
                ))}
                {bank.countries.length > 3 && (
                  <Text size="xs" c="dimmed">
                    +{bank.countries.length - 3}
                  </Text>
                )}
                <Text size="xs" c="dimmed" ff="monospace" truncate>
                  {bank.bic}
                </Text>
              </Group>
            }
          />
        ))}
      </Stack>
    </ComponentPreview>
  );
}

export function WithLogo() {
  const [selectedId, setSelectedId] = useState<string | null>(null);

  return (
    <ComponentPreview canvas={{ center: true, maxWidth: 320 }} withSpacing>
      <Stack gap="xs">
        <SelectionCard
          id="barclays"
          optionName="Barclays"
          logoUri={barclaysLogo}
          isSelected={selectedId === 'barclays'}
          onSelect={setSelectedId}
        />
        <SelectionCard
          id="revolut"
          optionName="Revolut"
          logoUri={revolutLogo}
          isSelected={selectedId === 'revolut'}
          onSelect={setSelectedId}
        />
      </Stack>
    </ComponentPreview>
  );
}
