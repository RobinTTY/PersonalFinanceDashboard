import { Group } from '@mantine/core';
import { ModalButton } from '../../../components/modal-button/ModalButton';
import { IconCoins, IconGraph } from '@tabler/icons-react';

import classes from './AccountTypeSelectionStep.module.css';

export const AccountTypeSelectionStep = ({ selectionAction }: AccountTypeSelectionStepProps) => {
  return (
    <Group className={classes.group} gap="lg" p="xs">
      {/* TODO: Create reusable component? */}
      <ModalButton
        icon={<IconCoins size="4rem" />}
        description="Savings Account"
        action={() => selectionAction('savings')}
      />
      <ModalButton
        icon={<IconGraph size="4rem" />}
        description="Investment Account"
        action={() => selectionAction('investment')}
      />
    </Group>
  );
};

export interface AccountTypeSelectionStepProps {
  selectionAction: (accountType: AccountType) => void;
}

export type AccountType = 'savings' | 'investment';
