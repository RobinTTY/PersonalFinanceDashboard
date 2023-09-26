import { gql } from '../types/gql';

export const GetBankingInstitutionsQuery = gql(`
  query GetBankingInstitutions{
    bankingInstitutions(first: 3000) {
      pageInfo{
        hasNextPage
        hasPreviousPage
        startCursor
        endCursor
      }
      edges {
        node {
          name
          bic
          logoUri
          countries
        }
      }
    }
  }
`);
