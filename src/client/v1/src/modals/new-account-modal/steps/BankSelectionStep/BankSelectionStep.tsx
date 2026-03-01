import { useQuery } from "@apollo/client/react";
import { Center, Loader } from '@mantine/core';
import { ModalOptionSearchList } from '@components/modal-option-search-list/ModalOptionSearchList';
import { GetBankingInstitutionsQuery } from '@/graphql/queries/GetBankingInstitutions';
import { BankSelectionStepProps } from './BankSelectionStepProps';

export const BankSelectionStep = ({ countryCode, onBankSelect }: BankSelectionStepProps) => {
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

  // TODO: replace with graphql query for specific country
  const institutions = data?.bankingInstitutions?.edges?.filter((edge) =>
    edge?.node?.countries.includes(countryCode)
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
