import { BankAccount } from '@graphql-types/graphql';
import { Authentication } from './AuthenticationStep';

export interface AccountSelection extends BankAccount {
  checked: boolean;
}

export interface AccountSelectionStepProps {
  authentication: Authentication;
  onAccountsSelected: (accounts: BankAccount[]) => void;
}
