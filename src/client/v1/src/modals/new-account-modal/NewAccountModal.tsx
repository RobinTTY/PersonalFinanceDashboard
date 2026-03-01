import { useState } from 'react';
import { Modal, Text } from '@mantine/core';
import { useCounter } from '@mantine/hooks';

import { AuthenticationRequest, BankAccount } from '@graphql-types/graphql';
import {
  AccountTypeSelectionStep,
  AccountType,
} from './steps/AccountTypeSelectionStep/AccountTypeSelectionStep';
import { CountrySelectionStep } from './steps/CountrySelectionStep/CountrySelectionStep';
import { BankSelectionStep } from './steps/BankSelectionStep/BankSelectionStep';
import { AuthenticationStep } from './steps/AuthenticationStep/AuthenticationStep';
import { AccountSelectionStep } from './steps/AccountSelectionStep/AccountSelectionStep';
import { AccountImportStep } from './steps/AccountImportStep/AccountImportStep';
import { NewAccountModalProps } from './NewAccountModalProps';

export const NewAccountModal = ({ opened, closeModal }: NewAccountModalProps) => {
  const [addAccountStep, addAccountStepHandler] = useCounter(1, {
    min: 1,
    max: 6,
  });
  const [accountType, setAccountType] = useState<AccountType>();
  const [countryCode, setCountryCode] = useState<string>();
  const [bankId, setBankId] = useState<string>();
  const [authentication, setAuthentication] = useState<AuthenticationRequest>();
  const [, setAccounts] = useState<BankAccount[]>();

  const closeTransitionDuration = 200;
  const closeAndResetModal = () => {
    closeModal();
    setTimeout(() => {
      addAccountStepHandler.reset();
    }, closeTransitionDuration);
  };

  const getActiveNewAccountStep = () => {
    switch (addAccountStep) {
      case 1:
        return (
          <AccountTypeSelectionStep
            selectionAction={(type) => {
              setAccountType(type);
              addAccountStepHandler.increment();
            }}
          />
        );
      case 2:
        switch (accountType) {
          case 'savings':
            return (
              <CountrySelectionStep
                onCountrySelect={(optionKey) => {
                  setCountryCode(optionKey);
                  addAccountStepHandler.increment();
                }}
              />
            );
          case 'investment':
            return <Text>Step 2</Text>;
          default:
            throw new Error('Invalid account type');
        }
      case 3:
        return (
          <BankSelectionStep
            countryCode={countryCode!}
            onBankSelect={(optionKey) => {
              setBankId(optionKey);
              addAccountStepHandler.increment();
            }}
          />
        );
      case 4:
        return (
          <AuthenticationStep
            bankId={bankId!}
            onFinishSetup={(auth) => {
              setAuthentication(auth);
              addAccountStepHandler.increment();
            }}
          />
        );
      case 5:
        return (
          <AccountSelectionStep
            authentication={authentication!}
            onAccountsSelected={(accounts) => {
              setAccounts(accounts);
              addAccountStepHandler.increment();
            }}
          />
        );
      case 6:
        return <AccountImportStep onImportComplete={closeAndResetModal} />;
      default:
        return <Text>Step {addAccountStep}</Text>;
    }
  };

  return (
    <Modal
      opened={opened}
      transitionProps={{ duration: closeTransitionDuration }}
      onClose={closeAndResetModal}
      title={
        <Text fz="xl" fw={700} pl=".4rem">
          {addAccountStep === 1 && 'What type of account would you like to add?'}
          {addAccountStep === 2 && 'Select your country'}
          {addAccountStep === 3 && 'Select your bank'}
          {addAccountStep === 4 && 'Authenticate'}
          {addAccountStep === 5 && 'Select the accounts to add'}
          {addAccountStep === 6 && 'Account import'}
        </Text>
      }
      centered
      size="auto"
    >
      {getActiveNewAccountStep()}
    </Modal>
  );
};
