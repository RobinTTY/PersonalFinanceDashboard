import { MockedResponse } from '@apollo/client/testing';
import { GetAccountsQuery } from '../../queries/GetAccounts';
import * as MockAccountData from './GetAccounts.json';

export const MockData: readonly MockedResponse<Record<string, any>, Record<string, any>>[] = [
  {
    request: {
      query: GetAccountsQuery,
      variables: { first: 5 },
    },
    result: {
      data: MockAccountData.data,
    },
  },
];
