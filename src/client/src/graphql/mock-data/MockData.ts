import { MockedResponse } from '@apollo/client/testing';
import { GetBankingInstitutionsQuery } from '../queries/GetBankingInstitutions';
import { GetAccountQuery } from '../queries/GetAccount';
import { GetAccountsQuery, GetMinimalAccountsQuery } from '../queries/GetAccounts';
import { CreateAuthenticationRequestMutation } from '../mutations/CreateAuthenticationRequest';
import { GetAuthenticationRequestQuery } from '../queries/GetAuthenticationRequest';
import { GetTransactionsQuery } from '../queries/GetTransactions';

import * as MockBankingInstitutionsData from './GetBankingInstitutions.json';
import * as MockAccountData from './GetAccount.json';
import * as MockAccountsData from './GetAccounts.json';
import * as MockMinimalAccountsData from './GetMinimalAccounts.json';
import * as MockCreateAuthenticationRequest from './CreateAuthenticationRequest.json';
import * as MockGetAuthenticationRequest from './GetAuthenticationRequest.json';
import * as MockGetTransactions from './GetTransactions.json';

const maxReuseCount = Number.POSITIVE_INFINITY;

// TODO: https://mswjs.io/docs/ has a more flexible testing approach, maybe migrate
export const MockData: readonly MockedResponse<Record<string, any>, Record<string, any>>[] = [
  {
    request: {
      query: GetBankingInstitutionsQuery,
      variables: {
        first: 3000,
      },
    },
    result: {
      data: MockBankingInstitutionsData.data,
    },
    maxUsageCount: maxReuseCount,
    delay: 1000,
  },
  {
    request: {
      query: GetAccountQuery,
      variables: {
        accountId: '072fefa4-4530-4322-aafe-e953d37402ae',
      },
    },
    result: {
      data: MockAccountData.data,
    },
    maxUsageCount: maxReuseCount,
    delay: 1000,
  },
  {
    request: {
      query: GetAccountsQuery,
      variables: {
        accountIds: [
          '072fefa4-4530-4322-aafe-e953d37402ae',
          '443a187b-6900-410e-8706-7cc0d8c39987',
        ],
      },
    },
    result: {
      data: MockAccountsData.data,
    },
    maxUsageCount: maxReuseCount,
  },
  // TODO: To be correct, this would require accountIds to be a non-empty array
  {
    request: {
      query: GetMinimalAccountsQuery,
      variables: {
        accountIds: [],
      },
    },
    result: {
      data: MockMinimalAccountsData.data,
    },
    maxUsageCount: maxReuseCount,
  },
  {
    request: {
      query: CreateAuthenticationRequestMutation,
      variables: {
        institutionId: 'SANDBOXFINANCE_SFIN0000',
        redirectUri: 'https://www.robintty.com/',
      },
    },
    result: {
      data: MockCreateAuthenticationRequest.data,
    },
    maxUsageCount: maxReuseCount,
  },
  {
    request: {
      query: GetAuthenticationRequestQuery,
      variables: {
        authenticationId: '57e19144-f1df-47ab-860b-c908fcd2015b',
      },
    },
    result: {
      data: MockGetAuthenticationRequest.data,
    },
    maxUsageCount: maxReuseCount,
    delay: 500,
  },
  {
    request: {
      query: GetTransactionsQuery,
      variables: {
        accountId: '072fefa4-4530-4322-aafe-e953d37402ae',
        first: 15,
      },
    },
    result: {
      data: MockGetTransactions.data,
    },
    maxUsageCount: maxReuseCount,
    delay: 500,
  },
];
