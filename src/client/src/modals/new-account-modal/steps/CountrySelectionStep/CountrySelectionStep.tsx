import {
  DE,
  US,
  CA,
  GB,
  AU,
  NZ,
  AT,
  BE,
  BG,
  CY,
  GR,
  CZ,
  DK,
  EE,
  ES,
  FI,
  FR,
  HR,
  HU,
  IE,
  IS,
  IT,
  LT,
  LU,
  LV,
  MT,
  NL,
  NO,
  PL,
  PT,
  RO,
  SE,
  SI,
  SK,
} from 'country-flag-icons/react/3x2';
import { ModalOptionSearchList } from '@components/modal-option-search-list/ModalOptionSearchList';
import { CountrySelectionStepProps } from './CountrySelectionStepProps';

const countries = [
  { key: 'US', description: 'United States', icon: <US /> },
  { key: 'CA', description: 'Canada', icon: <CA /> },
  { key: 'GB', description: 'United Kingdom', icon: <GB /> },
  { key: 'AU', description: 'Australia', icon: <AU /> },
  { key: 'NZ', description: 'New Zealand', icon: <NZ /> },
  { key: 'DE', description: 'Germany', icon: <DE /> },
  { key: 'AT', description: 'Austria', icon: <AT /> },
  { key: 'BE', description: 'Belgium', icon: <BE /> },
  { key: 'BG', description: 'Bulgaria', icon: <BG /> },
  { key: 'CY', description: 'Cyprus', icon: <CY /> },
  { key: 'CZ', description: 'Czech Republic', icon: <CZ /> },
  { key: 'DK', description: 'Denmark', icon: <DK /> },
  { key: 'EE', description: 'Estonia', icon: <EE /> },
  { key: 'ES', description: 'Spain', icon: <ES /> },
  { key: 'FI', description: 'Finland', icon: <FI /> },
  { key: 'FR', description: 'France', icon: <FR /> },
  { key: 'GR', description: 'Greece', icon: <GR /> },
  { key: 'HR', description: 'Croatia', icon: <HR /> },
  { key: 'HU', description: 'Hungary', icon: <HU /> },
  { key: 'IE', description: 'Ireland', icon: <IE /> },
  { key: 'IS', description: 'Iceland', icon: <IS /> },
  { key: 'IT', description: 'Italy', icon: <IT /> },
  { key: 'LT', description: 'Lithuania', icon: <LT /> },
  { key: 'LU', description: 'Luxembourg', icon: <LU /> },
  { key: 'LV', description: 'Latvia', icon: <LV /> },
  { key: 'MT', description: 'Malta', icon: <MT /> },
  { key: 'NL', description: 'Netherlands', icon: <NL /> },
  { key: 'NO', description: 'Norway', icon: <NO /> },
  { key: 'PL', description: 'Poland', icon: <PL /> },
  { key: 'PT', description: 'Portugal', icon: <PT /> },
  { key: 'RO', description: 'Romania', icon: <RO /> },
  { key: 'SE', description: 'Sweden', icon: <SE /> },
  { key: 'SI', description: 'Slovenia', icon: <SI /> },
  { key: 'SK', description: 'Slovakia', icon: <SK /> },
];

export const CountrySelectionStep = ({ onCountrySelect }: CountrySelectionStepProps) => (
  <ModalOptionSearchList
    options={countries}
    displayOptions={{ iconHeight: '32px', iconWidth: '48px' }}
    searchPlaceholder="Filter countries..."
    onOptionSelect={(optionKey) => {
      onCountrySelect(optionKey);
    }}
  />
);
