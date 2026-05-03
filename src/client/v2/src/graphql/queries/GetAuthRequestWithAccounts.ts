import { gql, TypedDocumentNode } from '@apollo/client';
import { GetAuthRequestWithAccountsQuery } from '../types/graphql';

export const GetAuthRequestWithAccounts: TypedDocumentNode<GetAuthRequestWithAccountsQuery> = gql(`
  query GetAuthRequestWithAccounts($authenticationId: UUID!) {
    authenticationRequest(authenticationId: $authenticationId) {
      id
      thirdPartyId
      status
      authenticationLink
      createdAt
      associatedAccounts{
        id
        thirdPartyId
        associatedInstitution{
          name
          bic
          logoUri
        }
      }
      associatedAccounts{
        id
        iban
        name
        description
        accountType      
        balance
        currency
        ownerName
      }
    }
  }
`);
