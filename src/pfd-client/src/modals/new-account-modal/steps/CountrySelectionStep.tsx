import { ModalOptionSearchList } from '@/components/modal-option-search-list/ModalOptionSearchList';
import { DE, US, CA, GB, AU, NZ } from 'country-flag-icons/react/3x2';

const countries = [
  { key: 'US', description: 'United States', icon: <US /> },
  { key: 'CA', description: 'Canada', icon: <CA /> },
  { key: 'GB', description: 'United Kingdom', icon: <GB /> },
  { key: 'AU', description: 'Australia', icon: <AU /> },
  { key: 'NZ', description: 'New Zealand', icon: <NZ /> },
  { key: 'DE', description: 'Germany', icon: <DE /> },
];

export const CountrySelectionStep = ({ onCountrySelect }: CountrySelectionStepProps) => {
  return (
    <ModalOptionSearchList
      options={countries}
      searchPlaceholder="Filter countries..."
      onOptionSelect={(optionKey) => {
        onCountrySelect(optionKey);
      }}
    />
  );
};

export interface CountrySelectionStepProps {
  onCountrySelect: (countryKey: string) => void;
}
