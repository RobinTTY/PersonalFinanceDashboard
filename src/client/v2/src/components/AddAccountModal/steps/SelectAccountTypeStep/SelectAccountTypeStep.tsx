import { IconBuildingBank, IconChartLine } from '@tabler/icons-react';
import { Group, Radio, SimpleGrid, Stack, Text, ThemeIcon } from '@mantine/core';
import classes from './SelectAccountTypeStep.module.css';

export type AccountType = 'savings' | 'investment';

interface AccountTypeOption {
  value: AccountType;
  label: string;
  description: string;
  icon: React.FC<{ size?: number }>;
}

const accountTypeOptions: AccountTypeOption[] = [
  {
    value: 'savings',
    label: 'Savings Account',
    description: 'Track your cash, checking, and savings balances.',
    icon: IconBuildingBank,
  },
  {
    value: 'investment',
    label: 'Investment Account',
    description: 'Track stocks, ETFs, and other investment portfolios.',
    icon: IconChartLine,
  },
];

interface SelectAccountTypeStepProps {
  selectedType: AccountType | undefined;
  onChange: (type: AccountType) => void;
}

export function SelectAccountTypeStep({ selectedType, onChange }: SelectAccountTypeStepProps) {
  return (
    <Radio.Group value={selectedType} onChange={(value) => onChange(value as AccountType)}>
      <SimpleGrid cols={2}>
        {accountTypeOptions.map((option) => (
          <Radio.Card key={option.value} value={option.value} className={classes.accountTypeCard}>
            <Group wrap="nowrap" align="center">
              <ThemeIcon size="xl" variant="light" radius="md" flex="0 0 auto">
                <option.icon size={20} />
              </ThemeIcon>
              <Stack gap={4}>
                <Text fw={600}>{option.label}</Text>
                <Text size="sm" c="dimmed">
                  {option.description}
                </Text>
              </Stack>
            </Group>
          </Radio.Card>
        ))}
      </SimpleGrid>
    </Radio.Group>
  );
}
