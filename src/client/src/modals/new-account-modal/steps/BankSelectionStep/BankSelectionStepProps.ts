export interface BankSelectionStepProps {
  countryCode: string;
  onBankSelect: (bankKey: string) => void;
}
