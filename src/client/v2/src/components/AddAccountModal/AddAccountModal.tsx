import { useState } from 'react';
import { IconBuildingBank, IconChartLine } from '@tabler/icons-react';
import {
  Button,
  CloseButton,
  Group,
  Modal,
  Radio,
  SimpleGrid,
  Stack,
  Text,
  ThemeIcon,
  Title,
} from '@mantine/core';
import classes from './AddAccountModal.module.css';

type AccountType = 'savings' | 'investment';

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

interface AddAccountModalProps {
  opened: boolean;
  onClose: () => void;
}

export function AddAccountModal({ opened, onClose }: AddAccountModalProps) {
  const [selectedType, setSelectedType] = useState<AccountType | undefined>(undefined);

  const handleClose = () => {
    setSelectedType(undefined);
    onClose();
  };

  return (
    <Modal.Root
      opened={opened}
      onClose={handleClose}
      size={700}
      padding={0}
      centered
      classNames={{
        content: classes.modalContent,
        body: classes.modalBody,
      }}
    >
      <Modal.Overlay />
      <Modal.Content aria-label="Add Account">
        <Modal.Body>
          <div className={classes.header}>
            <Stack gap={2}>
              <Title order={3}>Choose Account Type</Title>
              <Text size="sm" c="dimmed">Select the type of account you want to add.</Text>
            </Stack>
            <CloseButton onClick={handleClose} size="md" aria-label="Close" />
          </div>

          <div className={classes.content}>
            <Stack gap="lg">
              <Radio.Group
                value={selectedType}
                onChange={(value) => setSelectedType(value as AccountType)}
              >
                <SimpleGrid cols={2}>
                  {accountTypeOptions.map((option) => (
                    <Radio.Card
                      key={option.value}
                      value={option.value}
                      className={classes.accountTypeCard}
                    >
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
            </Stack>
          </div>

          <div className={classes.footer}>
            <Button variant="default" onClick={handleClose}>
              Cancel
            </Button>
            <Button disabled={!selectedType}>Next</Button>
          </div>
        </Modal.Body>
      </Modal.Content>
    </Modal.Root>
  );
}
