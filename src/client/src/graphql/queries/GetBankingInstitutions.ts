import { gql, TypedDocumentNode } from '@apollo/client';
import { GetBankingInstitutionsQuery } from '../types/graphql';

export const GetBankingInstitutions: TypedDocumentNode<GetBankingInstitutionsQuery> = gql(`
  query GetBankingInstitutions($countryCode: String $first: Int){
    bankingInstitutions(countryCode: $countryCode first: $first) {
      pageInfo{
        hasNextPage
        hasPreviousPage
        startCursor
        endCursor
      }
      edges {
        node {
          id
          thirdPartyId
          name
          bic
          logoUri
          countries
        }
      }
    }
  }
`);
