import { RequestHandler, graphql, rest } from 'msw';
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
  graphql.query<GetAccountQuery, GetAccountQueryVariables>('GetAccount', (req, res, ctx) => {
    const { accountId } = req.variables;
    console.log(accountId);

    ctx.delay(1000);
    return res(ctx.data(MockAccount.data as GetAccountQuery));
  }),

  graphql.query<GetMinimalAccountsQuery, GetMinimalAccountsQueryVariables>(
    'GetMinimalAccounts',
    (req, res, ctx) => {
      const { accountIds } = req.variables;
      console.log(accountIds);

      ctx.delay(1000);
      return res(ctx.data(MockMinimalAccounts.data as GetMinimalAccountsQuery));
    }
  ),

  graphql.query<GetAccountsQuery, GetAccountsQueryVariables>('GetAccounts', (req, res, ctx) => {
    const { accountIds } = req.variables;
    console.log(accountIds);

    ctx.delay(1000);
    return res(ctx.data(MockAccounts.data as GetAccountsQuery));
  }),

  graphql.query<GetBankingInstitutionsQuery, GetBankingInstitutionsQueryVariables>(
    'GetBankingInstitutions',
    (req, res, ctx) => {
      const { countryCode, first } = req.variables;
      console.log(countryCode, first);

      ctx.delay(defaultDelay);
      return res(ctx.data(MockBankingInstitutions.data as GetBankingInstitutionsQuery));
    }
  ),

  graphql.query<GetAuthenticationRequestQuery, GetAuthenticationRequestQueryVariables>(
    'GetAuthenticationRequest',
    (req, res, ctx) => {
      const { authenticationId } = req.variables;
      console.log(authenticationId);

      ctx.delay(defaultDelay);
      return res(ctx.data(MockGetAuthenticationRequest.data as GetAuthenticationRequestQuery));
    }
  ),

  graphql.query<GetTransactionsQueryWire, GetTransactionsQueryVariables>(
    'GetTransactions',
    (req, res, ctx) => {
      const { accountId, first } = req.variables;
      console.log(accountId, first);

      ctx.delay(defaultDelay);
      return res(ctx.data(MockGetTransactions.data as GetTransactionsQueryWire));
    }
  ),

  graphql.mutation<
    CreateAuthenticationRequestMutation,
    CreateAuthenticationRequestMutationVariables
  >('CreateAuthenticationRequest', (req, res, ctx) => {
    const { institutionId, redirectUri } = req.variables;
    console.log(institutionId, redirectUri);

    ctx.delay(defaultDelay);
    return res(
      ctx.data(MockCreateAuthenticationRequest.data as CreateAuthenticationRequestMutation)
    );
  }),

  // Ignore all get calls (non-graphql)
  rest.get('*', (req) => req.passthrough()),
];
