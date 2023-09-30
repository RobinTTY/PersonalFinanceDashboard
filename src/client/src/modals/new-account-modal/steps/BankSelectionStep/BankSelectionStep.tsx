import { useQuery } from '@apollo/client';
import { Center, Loader } from '@mantine/core';
import { ModalOptionSearchList } from '@components/modal-option-search-list/ModalOptionSearchList';
import { GetBankingInstitutionsQuery } from '@/graphql/queries/GetBankingInstitutions';
import { BankSelectionStepProps } from './BankSelectionStepProps';

// TODO: replace with grapql query
// const banks = [
//   {
//     key: 'DORTMUNDER_VOLKSBANK_GENODEM1DOR',
//     description: 'Dortmunder Volksbank',
//     icon: (
//       <img
//         src="https://cdn.nordigen.com/ais/VOLKSBANK_NIEDERGRAFSCHAFT_GENODEF1HOO.png"
//         alt="Dortmunder Volksbank"
//       />
//     ),
//   },
//   {
//     key: 'KSK_BOBLINGEN_BBKRDE6BXXX',
//     description: 'Kreissparkasse Böblingen',
//     icon: (
//       <img
//         src="https://storage.googleapis.com/gc-prd-institution_icons-production/DE/PNG/sparkasse.png"
//         alt="Kreissparkasse Böblingen"
//       />
//     ),
//   },
//   {
//     key: 'COMMERZBANK_COBADEFF',
//     description: 'Commerzbank',
//     icon: (
//       <img
//         src="https://storage.googleapis.com/gc-prd-institution_icons-production/DE/PNG/commerzbank.png"
//         alt="Commerzbank"
//       />
//     ),
//   },
//   {
//     key: 'ING_INGDDEFF',
//     description: 'ING',
//     icon: (
//       <img
//         src="https://storage.googleapis.com/gc-prd-institution_icons-production/DE/PNG/ing.png"
//         alt="ING"
//       />
//     ),
//   },
//   {
//     key: 'DKB_BYLADEM1',
//     description: 'Deutsche Kreditbank AG (DKB)',
//     icon: (
//       <img
//         src="https://storage.googleapis.com/gc-prd-institution_icons-production/DE/PNG/deutschekreditbank.png"
//         alt="Deutsche Kreditbank AG (DKB)"
//       />
//     ),
//   },
//   {
//     key: 'DEUTSCHE_BANK_DEUTDEFF',
//     description: 'Deutsche Bank',
//     icon: (
//       <img
//         src="https://storage.googleapis.com/gc-prd-institution_icons-production/DE/PNG/deutschebank.png"
//         alt="Deutsche Bank"
//       />
//     ),
//   },
// ];

// TODO: bad performance on modal option search for Germany (1200banks)
export const BankSelectionStep = ({ onBankSelect }: BankSelectionStepProps) => {
  const { loading, data } = useQuery(GetBankingInstitutionsQuery, {
    variables: { first: 3000 },
  });

  if (loading) {
    return (
      <Center h="100%">
        <Loader color="violet" />
      </Center>
    );
  }

  const institutions = data?.bankingInstitutions?.edges?.filter((edge) =>
    edge?.node?.countries.includes('DE')
  );
  const mappedData = institutions?.map((edge) => ({
    key: edge?.node?.id,
    description: edge?.node?.name,
    icon: <img src={edge?.node?.logoUri} alt={edge?.node?.name} />,
  }));

  return (
    <ModalOptionSearchList
      options={mappedData ?? []}
      searchPlaceholder="Filter banks..."
      optionDescriptionWidth={200}
      truncateOptionDescription
      onOptionSelect={(optionKey) => {
        onBankSelect(optionKey);
      }}
    />
  );
};
