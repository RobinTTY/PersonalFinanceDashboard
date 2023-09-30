import { gql } from '../types/gql';

export const GetBankingInstitutionsQuery = gql(`
  query GetBankingInstitutions($first: Int){
    bankingInstitutions(first: $first) {
      pageInfo{
        hasNextPage
        hasPreviousPage
        startCursor
        endCursor
      }
      edges {
        node {
          id
          name
          bic
          logoUri
          countries
        }
      }
    }
  }
`);
