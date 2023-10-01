/* eslint-disable */
import { TypedDocumentNode as DocumentNode } from '@graphql-typed-document-node/core';
export type Maybe<T> = T | null;
export type InputMaybe<T> = Maybe<T>;
export type Exact<T extends { [key: string]: unknown }> = { [K in keyof T]: T[K] };
export type MakeOptional<T, K extends keyof T> = Omit<T, K> & { [SubKey in K]?: Maybe<T[SubKey]> };
export type MakeMaybe<T, K extends keyof T> = Omit<T, K> & { [SubKey in K]: Maybe<T[SubKey]> };
export type MakeEmpty<T extends { [key: string]: unknown }, K extends keyof T> = { [_ in K]?: never };
export type Incremental<T> = T | { [P in keyof T]?: P extends ' $fragmentName' | '__typename' ? T[P] : never };
/** All built-in and custom scalars, mapped to their actual values */
export type Scalars = {
  ID: { input: string; output: string; }
  String: { input: string; output: string; }
  Boolean: { input: boolean; output: boolean; }
  Int: { input: number; output: number; }
  Float: { input: number; output: number; }
  /** The `DateTime` scalar represents an ISO-8601 compliant date time type. */
  DateTime: { input: Date; output: Date; }
  /** The built-in `Decimal` scalar type. */
  Decimal: { input: any; output: any; }
  URL: { input: any; output: any; }
};

/** A connection to a list of items. */
export type AccountsConnection = {
  __typename?: 'AccountsConnection';
  /** A list of edges. */
  edges?: Maybe<Array<AccountsEdge>>;
  /** A flattened list of the nodes. */
  nodes?: Maybe<Array<BankAccount>>;
  /** Information to aid in pagination. */
  pageInfo: PageInfo;
};

/** An edge in a connection. */
export type AccountsEdge = {
  __typename?: 'AccountsEdge';
  /** A cursor for use in pagination. */
  cursor: Scalars['String']['output'];
  /** The item at the end of the edge. */
  node: BankAccount;
};

export type AuthenticationRequest = {
  __typename?: 'AuthenticationRequest';
  associatedAccounts: Array<Scalars['String']['output']>;
  authenticationLink: Scalars['URL']['output'];
  id: Scalars['String']['output'];
  status: AuthenticationStatus;
};

export enum AuthenticationStatus {
  Expired = 'EXPIRED',
  Failed = 'FAILED',
  Pending = 'PENDING',
  RequiresUserAction = 'REQUIRES_USER_ACTION',
  Successful = 'SUCCESSFUL'
}

export type BankAccount = {
  __typename?: 'BankAccount';
  accountType?: Maybe<Scalars['String']['output']>;
  balance?: Maybe<Scalars['Decimal']['output']>;
  bban?: Maybe<Scalars['String']['output']>;
  bic?: Maybe<Scalars['String']['output']>;
  currency?: Maybe<Scalars['String']['output']>;
  description?: Maybe<Scalars['String']['output']>;
  iban?: Maybe<Scalars['String']['output']>;
  id: Scalars['String']['output'];
  name?: Maybe<Scalars['String']['output']>;
  ownerName?: Maybe<Scalars['String']['output']>;
  transactions: Array<Transaction>;
};

export type BankingInstitution = {
  __typename?: 'BankingInstitution';
  bic: Scalars['String']['output'];
  countries: Array<Scalars['String']['output']>;
  id: Scalars['String']['output'];
  logoUri: Scalars['URL']['output'];
  name: Scalars['String']['output'];
};

/** A connection to a list of items. */
export type BankingInstitutionsConnection = {
  __typename?: 'BankingInstitutionsConnection';
  /** A list of edges. */
  edges?: Maybe<Array<BankingInstitutionsEdge>>;
  /** A flattened list of the nodes. */
  nodes?: Maybe<Array<BankingInstitution>>;
  /** Information to aid in pagination. */
  pageInfo: PageInfo;
};

/** An edge in a connection. */
export type BankingInstitutionsEdge = {
  __typename?: 'BankingInstitutionsEdge';
  /** A cursor for use in pagination. */
  cursor: Scalars['String']['output'];
  /** The item at the end of the edge. */
  node: BankingInstitution;
};

export type BasicResponse = {
  __typename?: 'BasicResponse';
  details?: Maybe<Scalars['String']['output']>;
  summary?: Maybe<Scalars['String']['output']>;
};

/** GraphQL root type for mutation operations. */
export type Mutation = {
  __typename?: 'Mutation';
  /** TODO */
  createAuthenticationRequest: AuthenticationRequest;
  deleteAuthenticationRequest: BasicResponse;
};


/** GraphQL root type for mutation operations. */
export type MutationCreateAuthenticationRequestArgs = {
  institutionId: Scalars['String']['input'];
  redirectUri: Scalars['String']['input'];
};


/** GraphQL root type for mutation operations. */
export type MutationDeleteAuthenticationRequestArgs = {
  authenticationId: Scalars['String']['input'];
};

/** Information about pagination in a connection. */
export type PageInfo = {
  __typename?: 'PageInfo';
  /** When paginating forwards, the cursor to continue. */
  endCursor?: Maybe<Scalars['String']['output']>;
  /** Indicates whether more edges exist following the set defined by the clients arguments. */
  hasNextPage: Scalars['Boolean']['output'];
  /** Indicates whether more edges exist prior the set defined by the clients arguments. */
  hasPreviousPage: Scalars['Boolean']['output'];
  /** When paginating backwards, the cursor to continue. */
  startCursor?: Maybe<Scalars['String']['output']>;
};

/** GraphQL root type for query operations. */
export type Query = {
  __typename?: 'Query';
  /** Look up an account by its id. */
  account?: Maybe<BankAccount>;
  /** Look up accounts by a list of ids. */
  accounts?: Maybe<AccountsConnection>;
  authenticationRequest?: Maybe<AuthenticationRequest>;
  authenticationRequests: Array<AuthenticationRequest>;
  bankingInstitutions?: Maybe<BankingInstitutionsConnection>;
  /** Look up transactions of an account. */
  transactions?: Maybe<TransactionsConnection>;
};


/** GraphQL root type for query operations. */
export type QueryAccountArgs = {
  accountId: Scalars['String']['input'];
};


/** GraphQL root type for query operations. */
export type QueryAccountsArgs = {
  accountIds: Array<Scalars['String']['input']>;
  after?: InputMaybe<Scalars['String']['input']>;
  before?: InputMaybe<Scalars['String']['input']>;
  first?: InputMaybe<Scalars['Int']['input']>;
  last?: InputMaybe<Scalars['Int']['input']>;
};


/** GraphQL root type for query operations. */
export type QueryAuthenticationRequestArgs = {
  authenticationId: Scalars['String']['input'];
};


/** GraphQL root type for query operations. */
export type QueryBankingInstitutionsArgs = {
  after?: InputMaybe<Scalars['String']['input']>;
  before?: InputMaybe<Scalars['String']['input']>;
  first?: InputMaybe<Scalars['Int']['input']>;
  last?: InputMaybe<Scalars['Int']['input']>;
};


/** GraphQL root type for query operations. */
export type QueryTransactionsArgs = {
  accountId: Scalars['String']['input'];
  after?: InputMaybe<Scalars['String']['input']>;
  before?: InputMaybe<Scalars['String']['input']>;
  first?: InputMaybe<Scalars['Int']['input']>;
  last?: InputMaybe<Scalars['Int']['input']>;
};

export type Transaction = {
  __typename?: 'Transaction';
  amount: Scalars['Decimal']['output'];
  category: Scalars['String']['output'];
  currency: Scalars['String']['output'];
  id: Scalars['String']['output'];
  notes: Scalars['String']['output'];
  payee?: Maybe<Scalars['String']['output']>;
  payer?: Maybe<Scalars['String']['output']>;
  tags: Array<Scalars['String']['output']>;
  valueDate?: Maybe<Scalars['DateTime']['output']>;
};

/** A connection to a list of items. */
export type TransactionsConnection = {
  __typename?: 'TransactionsConnection';
  /** A list of edges. */
  edges?: Maybe<Array<TransactionsEdge>>;
  /** A flattened list of the nodes. */
  nodes?: Maybe<Array<Transaction>>;
  /** Information to aid in pagination. */
  pageInfo: PageInfo;
};

/** An edge in a connection. */
export type TransactionsEdge = {
  __typename?: 'TransactionsEdge';
  /** A cursor for use in pagination. */
  cursor: Scalars['String']['output'];
  /** The item at the end of the edge. */
  node: Transaction;
};

export type CreateAuthenticationRequestMutationVariables = Exact<{
  institutionId: Scalars['String']['input'];
  redirectUri: Scalars['String']['input'];
}>;


export type CreateAuthenticationRequestMutation = { __typename?: 'Mutation', createAuthenticationRequest: { __typename?: 'AuthenticationRequest', id: string, associatedAccounts: Array<string>, status: AuthenticationStatus, authenticationLink: any } };

export type GetAccountQueryVariables = Exact<{
  accountId: Scalars['String']['input'];
}>;


export type GetAccountQuery = { __typename?: 'Query', account?: { __typename?: 'BankAccount', name?: string | null, description?: string | null, balance?: any | null, currency?: string | null, iban?: string | null, bic?: string | null, bban?: string | null, ownerName?: string | null, accountType?: string | null } | null };

export type GetAccountsQueryVariables = Exact<{
  accountIds: Array<Scalars['String']['input']> | Scalars['String']['input'];
}>;


export type GetAccountsQuery = { __typename?: 'Query', accounts?: { __typename?: 'AccountsConnection', edges?: Array<{ __typename?: 'AccountsEdge', node: { __typename?: 'BankAccount', id: string, name?: string | null, description?: string | null, balance?: any | null, currency?: string | null, iban?: string | null, bic?: string | null, bban?: string | null, ownerName?: string | null, accountType?: string | null } }> | null } | null };

export type GetMinimalAccountsQueryVariables = Exact<{
  accountIds: Array<Scalars['String']['input']> | Scalars['String']['input'];
}>;


export type GetMinimalAccountsQuery = { __typename?: 'Query', accounts?: { __typename?: 'AccountsConnection', edges?: Array<{ __typename?: 'AccountsEdge', node: { __typename?: 'BankAccount', id: string, description?: string | null, balance?: any | null, currency?: string | null } }> | null } | null };

export type GetAuthenticationRequestQueryVariables = Exact<{
  authenticationId: Scalars['String']['input'];
}>;


export type GetAuthenticationRequestQuery = { __typename?: 'Query', authenticationRequest?: { __typename?: 'AuthenticationRequest', id: string, associatedAccounts: Array<string>, status: AuthenticationStatus, authenticationLink: any } | null };

export type GetBankingInstitutionsQueryVariables = Exact<{
  first?: InputMaybe<Scalars['Int']['input']>;
}>;


export type GetBankingInstitutionsQuery = { __typename?: 'Query', bankingInstitutions?: { __typename?: 'BankingInstitutionsConnection', pageInfo: { __typename?: 'PageInfo', hasNextPage: boolean, hasPreviousPage: boolean, startCursor?: string | null, endCursor?: string | null }, edges?: Array<{ __typename?: 'BankingInstitutionsEdge', node: { __typename?: 'BankingInstitution', id: string, name: string, bic: string, logoUri: any, countries: Array<string> } }> | null } | null };

export type GetTransactionsQueryVariables = Exact<{
  accountId: Scalars['String']['input'];
  first?: InputMaybe<Scalars['Int']['input']>;
}>;


export type GetTransactionsQuery = { __typename?: 'Query', transactions?: { __typename?: 'TransactionsConnection', pageInfo: { __typename?: 'PageInfo', hasNextPage: boolean, hasPreviousPage: boolean, startCursor?: string | null, endCursor?: string | null }, edges?: Array<{ __typename?: 'TransactionsEdge', node: { __typename?: 'Transaction', valueDate?: Date | null, payer?: string | null, payee?: string | null, amount: any, currency: string, category: string, tags: Array<string>, notes: string } }> | null } | null };


export const CreateAuthenticationRequestDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"mutation","name":{"kind":"Name","value":"CreateAuthenticationRequest"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"institutionId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"redirectUri"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"createAuthenticationRequest"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"institutionId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"institutionId"}}},{"kind":"Argument","name":{"kind":"Name","value":"redirectUri"},"value":{"kind":"Variable","name":{"kind":"Name","value":"redirectUri"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"id"}},{"kind":"Field","name":{"kind":"Name","value":"associatedAccounts"}},{"kind":"Field","name":{"kind":"Name","value":"status"}},{"kind":"Field","name":{"kind":"Name","value":"authenticationLink"}}]}}]}}]} as unknown as DocumentNode<CreateAuthenticationRequestMutation, CreateAuthenticationRequestMutationVariables>;
export const GetAccountDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetAccount"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"accountId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"account"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"accountId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"accountId"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"name"}},{"kind":"Field","name":{"kind":"Name","value":"description"}},{"kind":"Field","name":{"kind":"Name","value":"balance"}},{"kind":"Field","name":{"kind":"Name","value":"currency"}},{"kind":"Field","name":{"kind":"Name","value":"iban"}},{"kind":"Field","name":{"kind":"Name","value":"bic"}},{"kind":"Field","name":{"kind":"Name","value":"bban"}},{"kind":"Field","name":{"kind":"Name","value":"ownerName"}},{"kind":"Field","name":{"kind":"Name","value":"accountType"}}]}}]}}]} as unknown as DocumentNode<GetAccountQuery, GetAccountQueryVariables>;
export const GetAccountsDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetAccounts"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"accountIds"}},"type":{"kind":"NonNullType","type":{"kind":"ListType","type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"accounts"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"accountIds"},"value":{"kind":"Variable","name":{"kind":"Name","value":"accountIds"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"edges"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"node"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"id"}},{"kind":"Field","name":{"kind":"Name","value":"name"}},{"kind":"Field","name":{"kind":"Name","value":"description"}},{"kind":"Field","name":{"kind":"Name","value":"balance"}},{"kind":"Field","name":{"kind":"Name","value":"currency"}},{"kind":"Field","name":{"kind":"Name","value":"iban"}},{"kind":"Field","name":{"kind":"Name","value":"bic"}},{"kind":"Field","name":{"kind":"Name","value":"bban"}},{"kind":"Field","name":{"kind":"Name","value":"ownerName"}},{"kind":"Field","name":{"kind":"Name","value":"accountType"}}]}}]}}]}}]}}]} as unknown as DocumentNode<GetAccountsQuery, GetAccountsQueryVariables>;
export const GetMinimalAccountsDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetMinimalAccounts"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"accountIds"}},"type":{"kind":"NonNullType","type":{"kind":"ListType","type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"accounts"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"accountIds"},"value":{"kind":"Variable","name":{"kind":"Name","value":"accountIds"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"edges"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"node"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"id"}},{"kind":"Field","name":{"kind":"Name","value":"description"}},{"kind":"Field","name":{"kind":"Name","value":"balance"}},{"kind":"Field","name":{"kind":"Name","value":"currency"}}]}}]}}]}}]}}]} as unknown as DocumentNode<GetMinimalAccountsQuery, GetMinimalAccountsQueryVariables>;
export const GetAuthenticationRequestDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetAuthenticationRequest"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"authenticationId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"authenticationRequest"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"authenticationId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"authenticationId"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"id"}},{"kind":"Field","name":{"kind":"Name","value":"associatedAccounts"}},{"kind":"Field","name":{"kind":"Name","value":"status"}},{"kind":"Field","name":{"kind":"Name","value":"authenticationLink"}}]}}]}}]} as unknown as DocumentNode<GetAuthenticationRequestQuery, GetAuthenticationRequestQueryVariables>;
export const GetBankingInstitutionsDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetBankingInstitutions"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"first"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"Int"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"bankingInstitutions"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"first"},"value":{"kind":"Variable","name":{"kind":"Name","value":"first"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"pageInfo"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"hasNextPage"}},{"kind":"Field","name":{"kind":"Name","value":"hasPreviousPage"}},{"kind":"Field","name":{"kind":"Name","value":"startCursor"}},{"kind":"Field","name":{"kind":"Name","value":"endCursor"}}]}},{"kind":"Field","name":{"kind":"Name","value":"edges"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"node"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"id"}},{"kind":"Field","name":{"kind":"Name","value":"name"}},{"kind":"Field","name":{"kind":"Name","value":"bic"}},{"kind":"Field","name":{"kind":"Name","value":"logoUri"}},{"kind":"Field","name":{"kind":"Name","value":"countries"}}]}}]}}]}}]}}]} as unknown as DocumentNode<GetBankingInstitutionsQuery, GetBankingInstitutionsQueryVariables>;
export const GetTransactionsDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"GetTransactions"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"accountId"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}},{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"first"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"Int"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"transactions"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"accountId"},"value":{"kind":"Variable","name":{"kind":"Name","value":"accountId"}}},{"kind":"Argument","name":{"kind":"Name","value":"first"},"value":{"kind":"Variable","name":{"kind":"Name","value":"first"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"pageInfo"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"hasNextPage"}},{"kind":"Field","name":{"kind":"Name","value":"hasPreviousPage"}},{"kind":"Field","name":{"kind":"Name","value":"startCursor"}},{"kind":"Field","name":{"kind":"Name","value":"endCursor"}}]}},{"kind":"Field","name":{"kind":"Name","value":"edges"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"node"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"valueDate"}},{"kind":"Field","name":{"kind":"Name","value":"payer"}},{"kind":"Field","name":{"kind":"Name","value":"payee"}},{"kind":"Field","name":{"kind":"Name","value":"amount"}},{"kind":"Field","name":{"kind":"Name","value":"currency"}},{"kind":"Field","name":{"kind":"Name","value":"category"}},{"kind":"Field","name":{"kind":"Name","value":"tags"}},{"kind":"Field","name":{"kind":"Name","value":"notes"}}]}}]}}]}}]}}]} as unknown as DocumentNode<GetTransactionsQuery, GetTransactionsQueryVariables>;