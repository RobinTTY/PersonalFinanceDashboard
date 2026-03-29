import { useState } from 'react';
import { Stack } from '@mantine/core';
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
