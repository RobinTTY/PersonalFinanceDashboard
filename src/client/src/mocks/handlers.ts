import type {
  CreateAuthenticationRequestMutation,
  CreateAuthenticationRequestMutationVariables,
  DeleteAuthenticationRequestMutation,
  DeleteAuthenticationRequestMutationVariables,
  GetAuthenticationRequestQueryVariables,
  GetAuthRequestWithAccountsQueryVariables,
  GetBankAccountQuery,
  GetBankAccountQueryVariables,
  GetTransactionsByAccountIdQuery,
  GetTransactionsByAccountIdQueryVariables,
  SetBankAccountIncludeInAnalyticsMutation,
  SetBankAccountIncludeInAnalyticsMutationVariables,
} from '@graphql-types/graphql';
import { graphql, HttpResponse } from 'msw';
import { authenticationRequestsData } from './data/authRequests';
import { bankAccountDetailsById } from './data/bankAccount';
import { bankAccountsData } from './data/bankAccounts';
import { dashboardData } from './data/dashboard';
import { bankingInstitutionsData } from './data/institutions';
import { transactionsFirstPage } from './data/transactions';

/**
 * MSW request handlers for the GraphQL API.
 *
 * Handlers are matched by GraphQL operation name, independent of the endpoint URL, so
 * they intercept the requests Apollo Client sends regardless of which `uri` it targets.
 * Handlers that take an id return the matching fixture and fall back to the first record
 * when the id is unknown, so any argument still yields a usable response.
 */
export const handlers = [
  graphql.query('GetNetWorthHistory', () => HttpResponse.json({ data: dashboardData })),

  graphql.query('GetBankAccounts', () => HttpResponse.json({ data: bankAccountsData })),

  graphql.query('GetBankingInstitutions', () =>
    HttpResponse.json({ data: bankingInstitutionsData })
  ),

  graphql.query<GetBankAccountQuery, GetBankAccountQueryVariables>(
    'GetBankAccount',
    ({ variables }) => {
      const details = Object.values(bankAccountDetailsById);
      const bankAccount = bankAccountDetailsById[String(variables.accountId)] ?? details[0] ?? null;
      return HttpResponse.json({ data: { bankAccount } });
    }
  ),

  graphql.query<GetTransactionsByAccountIdQuery, GetTransactionsByAccountIdQueryVariables>(
    'GetTransactionsByAccountId',
    ({ variables }) => {
      // The fixture is a single page. For paginated follow-up requests (`after`), return
      // an empty page so `fetchMore` terminates instead of looping on the same data.
      if (variables.after) {
        return HttpResponse.json({
          data: {
            transactionsByAccountId: {
              __typename: 'TransactionsByAccountIdConnection',
              pageInfo: {
                __typename: 'PageInfo',
                hasNextPage: false,
                startCursor: null,
                endCursor: null,
              },
              edges: [],
            },
          },
        });
      }
      return HttpResponse.json({ data: transactionsFirstPage });
    }
  ),

  graphql.query('GetAuthRequestsWithAccounts', () =>
    HttpResponse.json({ data: { authenticationRequests: authenticationRequestsData } })
  ),

  graphql.query<
    { authenticationRequest: (typeof authenticationRequestsData)[number] | null },
    GetAuthRequestWithAccountsQueryVariables
  >('GetAuthRequestWithAccounts', ({ variables }) => {
    const authenticationRequest =
      authenticationRequestsData.find((request) => request.id === variables.authenticationId) ??
      authenticationRequestsData[0] ??
      null;
    return HttpResponse.json({ data: { authenticationRequest } });
  }),

  // The narrower GetAuthenticationRequest selection is satisfied by the same records;
  // Apollo reads only the fields its query asks for, so extra fixture fields are harmless.
  graphql.query<
    { authenticationRequest: (typeof authenticationRequestsData)[number] | null },
    GetAuthenticationRequestQueryVariables
  >('GetAuthenticationRequest', ({ variables }) => {
    const authenticationRequest =
      authenticationRequestsData.find((request) => request.id === variables.authenticationId) ??
      authenticationRequestsData[0] ??
      null;
    return HttpResponse.json({ data: { authenticationRequest } });
  }),

  graphql.mutation<
    CreateAuthenticationRequestMutation,
    CreateAuthenticationRequestMutationVariables
  >('CreateAuthenticationRequest', ({ variables }) => {
    const data = {
      createAuthenticationRequest: {
        __typename: 'CreateAuthenticationRequestPayload',
        authenticationRequest: {
          __typename: 'AuthenticationRequest',
          id: 'a0000000-0000-0000-0000-000000000000',
          thirdPartyId: 'b0000000-0000-0000-0000-000000000000',
          status: 'CREATED',
          authenticationLink: `https://ob.gocardless.com/ob-psd2/start/00000000-0000-0000-0000-000000000000/${variables.institutionId}`,
          associatedAccounts: [],
        },
      },
    };
    return HttpResponse.json({ data: data as unknown as CreateAuthenticationRequestMutation });
  }),

  graphql.mutation<
    DeleteAuthenticationRequestMutation,
    DeleteAuthenticationRequestMutationVariables
  >('DeleteAuthenticationRequest', ({ variables }) =>
    HttpResponse.json({
      data: {
        deleteAuthenticationRequest: {
          __typename: 'DeleteAuthenticationRequestPayload',
          deleteResponse: {
            __typename: 'DeleteResponse',
            deletedId: variables.authenticationId,
          },
        },
      },
    })
  ),

  graphql.mutation<
    SetBankAccountIncludeInAnalyticsMutation,
    SetBankAccountIncludeInAnalyticsMutationVariables
  >('SetBankAccountIncludeInAnalytics', ({ variables }) =>
    HttpResponse.json({
      data: {
        setBankAccountIncludeInAnalytics: {
          __typename: 'SetBankAccountIncludeInAnalyticsPayload',
          bankAccount: {
            __typename: 'BankAccount',
            id: variables.bankAccountId,
            includeInAnalytics: variables.includeInAnalytics,
          },
        },
      },
    })
  ),
];
