import { MockedResponse } from '@apollo/client/testing';
import { GetAccountQuery } from '../queries/GetAccount';
import { GetAccountsQuery, GetMinimalAccountsQuery } from '../queries/GetAccounts';
import { CreateAuthenticationRequestMutation } from '../mutations/CreateAuthenticationRequest';
import { GetAuthenticationRequestQuery } from '../queries/GetAuthenticationRequest';

import * as MockAccountData from './GetAccount.json';
import * as MockAccountsData from './GetAccounts.json';
import * as MockMinimalAccountsData from './GetMinimalAccounts.json';
import * as MockCreateAuthenticationRequest from './CreateAuthenticationRequest.json';
import * as MockGetAuthenticationRequest from './GetAuthenticationRequest.json';

const maxReuseCount = Number.POSITIVE_INFINITY;

export const MockData: readonly MockedResponse<Record<string, any>, Record<string, any>>[] = [
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
];
