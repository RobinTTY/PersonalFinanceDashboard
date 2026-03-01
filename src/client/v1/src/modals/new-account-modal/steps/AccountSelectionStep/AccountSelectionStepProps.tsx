import { AuthenticationRequest, BankAccount } from '@graphql-types/graphql';

export interface AccountSelection extends BankAccount {
  checked: boolean;
}

export interface AccountSelectionStepProps {
  authentication: AuthenticationRequest;
  onAccountsSelected: (accounts: BankAccount[]) => void;
}
