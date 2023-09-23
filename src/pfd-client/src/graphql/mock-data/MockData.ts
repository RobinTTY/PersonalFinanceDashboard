import { MockedResponse } from '@apollo/client/testing';
import { GetAccountsQuery } from '../queries/GetAccounts';
import { CreateAuthenticationRequestMutation } from '../mutations/CreateAuthenticationRequest';

import * as MockAccountData from './GetAccounts.json';
import * as MockCreateAuthenticationRequest from './CreateAuthenticationRequest.json';

const maxReuseCount = Number.POSITIVE_INFINITY;

export const MockData: readonly MockedResponse<Record<string, any>, Record<string, any>>[] = [
  {
    request: {
      query: GetAccountsQuery,
      variables: { first: 5 },
    },
    result: {
      data: MockAccountData.data,
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
];
