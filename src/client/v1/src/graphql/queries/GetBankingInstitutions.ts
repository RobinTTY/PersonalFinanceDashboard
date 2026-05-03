import { gql } from '../types/gql';

export const GetBankingInstitutionsQuery = gql(`
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
          name
          bic
          logoUri
          countries
        }
      }
    }
  }
`);
