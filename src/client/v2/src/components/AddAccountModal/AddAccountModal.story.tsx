import { useState } from 'react';
import { MockedProvider } from '@apollo/client/testing/react';
import { Button } from '@mantine/core';
import { GetBankingInstitutions } from '@graphql-queries/GetBankingInstitutions';
import { ComponentPreview } from '../storybook/ComponentPreview';
import { AddAccountModal } from './AddAccountModal';

const bankingInstitutionsMock = {
  request: { query: GetBankingInstitutions, variables: { first: 3000 } },
  result: {
    data: {
      bankingInstitutions: {
        pageInfo: { hasNextPage: false, hasPreviousPage: false, startCursor: 'MA==', endCursor: 'MTIxMQ==' },
        edges: [
          { node: { id: 'DIREKT_HELADEF1822', name: '1822direkt', bic: 'HELADEF1822', logoUri: null } },
          { node: { id: 'AACHENER_BANK_GENODED1AAC', name: 'Aachener Bank', bic: 'GENODED1AAC', logoUri: null } },
          { node: { id: 'COMMERZBANK_COBADEFFXXX', name: 'Commerzbank', bic: 'COBADEFFXXX', logoUri: null } },
        ],
      },
    },
  },
};

export default { title: 'Accounts/AddAccountModal' };

export function Usage() {
  const [opened, setOpened] = useState(false);

  return (
    <MockedProvider mocks={[bankingInstitutionsMock]}>
      <ComponentPreview canvas={{ center: true }} withSpacing>
        <Button onClick={() => setOpened(true)}>Add Account</Button>
        <AddAccountModal opened={opened} onClose={() => setOpened(false)} />
      </ComponentPreview>
    </MockedProvider>
  );
}

export function OpenByDefault() {
  const [opened, setOpened] = useState(true);

  return (
    <MockedProvider mocks={[bankingInstitutionsMock]}>
      <ComponentPreview canvas={{ center: true }} withSpacing>
        <Button onClick={() => setOpened(true)}>Add Account</Button>
        <AddAccountModal opened={opened} onClose={() => setOpened(false)} />
      </ComponentPreview>
    </MockedProvider>
  );
}
