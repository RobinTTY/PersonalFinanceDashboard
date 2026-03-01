import { RequestHandler, graphql, delay, HttpResponse, http, passthrough } from 'msw';
import {
  GetAccountsQueryVariables,
  GetAccountsQuery,
  GetBankingInstitutionsQuery,
  GetBankingInstitutionsQueryVariables,
  GetMinimalAccountsQuery,
  GetMinimalAccountsQueryVariables,
  GetAccountQuery,
  GetAccountQueryVariables,
  GetAuthenticationRequestQuery,
  GetAuthenticationRequestQueryVariables,
  GetTransactionsQueryVariables,
  CreateAuthenticationRequestMutation,
  CreateAuthenticationRequestMutationVariables,
} from '@graphql-types/graphql';

import * as MockAccount from './GetAccount.json';
import * as MockMinimalAccounts from './GetMinimalAccounts.json';
import * as MockAccounts from './GetAccounts.json';
import * as MockBankingInstitutions from './GetBankingInstitutions.json';
import * as MockGetAuthenticationRequest from './GetAuthenticationRequest.json';
import * as MockGetTransactions from './GetTransactions.json';
import * as MockCreateAuthenticationRequest from './CreateAuthenticationRequest.json';
import { GetTransactionsQueryWire } from './wire-types';

const defaultDelay = 500;
export const handlers: Array<RequestHandler> = [
  graphql.query<GetAccountQuery, GetAccountQueryVariables>('GetAccount', async ({ variables }) => {
    const { accountId } = variables;
    console.log(accountId);

    await delay(1000);
    return HttpResponse.json({
      data: MockAccount.data as GetAccountQuery,
    });
  }),

  graphql.query<GetMinimalAccountsQuery, GetMinimalAccountsQueryVariables>(
    'GetMinimalAccounts',
    async ({ variables }) => {
      const { accountIds } = variables;
      console.log(accountIds);

      await delay(1000);
      return HttpResponse.json({
        data: MockMinimalAccounts.data as GetMinimalAccountsQuery,
      });
    }
  ),

  graphql.query<GetAccountsQuery, GetAccountsQueryVariables>(
    'GetAccounts',
    async ({ variables }) => {
      const { accountIds } = variables;
      console.log(accountIds);

      await delay(1000);
      return HttpResponse.json({
        data: MockAccounts.data as GetAccountsQuery,
      });
    }
  ),

  graphql.query<GetBankingInstitutionsQuery, GetBankingInstitutionsQueryVariables>(
    'GetBankingInstitutions',
    async ({ variables }) => {
      const { countryCode, first } = variables;
      console.log(countryCode, first);

      await delay(defaultDelay);
      return HttpResponse.json({
        data: MockBankingInstitutions.data as GetBankingInstitutionsQuery,
      });
    }
  ),

  graphql.query<GetAuthenticationRequestQuery, GetAuthenticationRequestQueryVariables>(
    'GetAuthenticationRequest',
    async ({ variables }) => {
      const { authenticationId } = variables;
      console.log(authenticationId);

      await delay(defaultDelay);
      return HttpResponse.json({
        data: MockGetAuthenticationRequest.data as GetAuthenticationRequestQuery,
      });
    }
  ),

  graphql.query<GetTransactionsQueryWire, GetTransactionsQueryVariables>(
    'GetTransactions',
    async ({ variables }) => {
      const { accountId, first } = variables;
      console.log(accountId, first);

      await delay(defaultDelay);
      return HttpResponse.json({
        data: MockGetTransactions.data as GetTransactionsQueryWire,
      });
    }
  ),

  graphql.mutation<
    CreateAuthenticationRequestMutation,
    CreateAuthenticationRequestMutationVariables
  >('CreateAuthenticationRequest', async ({ variables }) => {
    const { institutionId, redirectUri } = variables;
    console.log(institutionId, redirectUri);

    await delay(defaultDelay);
    return HttpResponse.json({
      data: MockCreateAuthenticationRequest.data as CreateAuthenticationRequestMutation,
    });
  }),

  // Don't pass through any unhandled graphql operations
  graphql.operation((req) => {
    throw new Error(`Unhandled graphql operation: ${req.operationName}`);
  }),

  // Ignore all get calls (non-graphql)
  http.get('*', () => passthrough()),
];
