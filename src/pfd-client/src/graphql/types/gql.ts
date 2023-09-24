/* eslint-disable */
import * as types from './graphql';
import { TypedDocumentNode as DocumentNode } from '@graphql-typed-document-node/core';

/**
 * Map of all GraphQL operations in the project.
 *
 * This map has several performance disadvantages:
 * 1. It is not tree-shakeable, so it will include all operations in the project.
 * 2. It is not minifiable, so the string of a GraphQL query will be multiple times inside the bundle.
 * 3. It does not support dead code elimination, so it will add unused operations.
 *
 * Therefore it is highly recommended to use the babel or swc plugin for production.
 */
const documents = {
    "\n  mutation CreateAuthenticationRequest(\n    $institutionId: String!\n    $redirectUri: String!\n  ) {\n    createAuthenticationRequest(\n      institutionId: $institutionId\n      redirectUri: $redirectUri\n    ) {\n      id\n      associatedAccounts\n      status\n      authenticationLink\n    }\n  }\n": types.CreateAuthenticationRequestDocument,
    "\n  query GetAccount($accountId: String!) {\n    account(accountId: $accountId) {\n      name\n      description\n      balance\n      currency\n      iban\n      bic\n      bban\n      ownerName\n      accountType\n    }\n  }\n": types.GetAccountDocument,
    "\n  query GetAccounts($first: Int) {\n    accounts(first: $first) {\n      edges {\n        node {\n          description\n          balance\n          currency\n        }\n      }\n    }\n  }\n": types.GetAccountsDocument,
    "\n  query GetAuthenticationRequest($authenticationId: String!) {\n    authenticationRequest(authenticationId: $authenticationId) {\n      id\n      associatedAccounts\n      status\n      authenticationLink\n    }\n  }\n": types.GetAuthenticationRequestDocument,
};

/**
 * The gql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 *
 *
 * @example
 * ```ts
 * const query = gql(`query GetUser($id: ID!) { user(id: $id) { name } }`);
 * ```
 *
 * The query argument is unknown!
 * Please regenerate the types.
 */
export function gql(source: string): unknown;

/**
 * The gql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function gql(source: "\n  mutation CreateAuthenticationRequest(\n    $institutionId: String!\n    $redirectUri: String!\n  ) {\n    createAuthenticationRequest(\n      institutionId: $institutionId\n      redirectUri: $redirectUri\n    ) {\n      id\n      associatedAccounts\n      status\n      authenticationLink\n    }\n  }\n"): (typeof documents)["\n  mutation CreateAuthenticationRequest(\n    $institutionId: String!\n    $redirectUri: String!\n  ) {\n    createAuthenticationRequest(\n      institutionId: $institutionId\n      redirectUri: $redirectUri\n    ) {\n      id\n      associatedAccounts\n      status\n      authenticationLink\n    }\n  }\n"];
/**
 * The gql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function gql(source: "\n  query GetAccount($accountId: String!) {\n    account(accountId: $accountId) {\n      name\n      description\n      balance\n      currency\n      iban\n      bic\n      bban\n      ownerName\n      accountType\n    }\n  }\n"): (typeof documents)["\n  query GetAccount($accountId: String!) {\n    account(accountId: $accountId) {\n      name\n      description\n      balance\n      currency\n      iban\n      bic\n      bban\n      ownerName\n      accountType\n    }\n  }\n"];
/**
 * The gql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function gql(source: "\n  query GetAccounts($first: Int) {\n    accounts(first: $first) {\n      edges {\n        node {\n          description\n          balance\n          currency\n        }\n      }\n    }\n  }\n"): (typeof documents)["\n  query GetAccounts($first: Int) {\n    accounts(first: $first) {\n      edges {\n        node {\n          description\n          balance\n          currency\n        }\n      }\n    }\n  }\n"];
/**
 * The gql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function gql(source: "\n  query GetAuthenticationRequest($authenticationId: String!) {\n    authenticationRequest(authenticationId: $authenticationId) {\n      id\n      associatedAccounts\n      status\n      authenticationLink\n    }\n  }\n"): (typeof documents)["\n  query GetAuthenticationRequest($authenticationId: String!) {\n    authenticationRequest(authenticationId: $authenticationId) {\n      id\n      associatedAccounts\n      status\n      authenticationLink\n    }\n  }\n"];

export function gql(source: string) {
  return (documents as any)[source] ?? {};
}

export type DocumentType<TDocumentNode extends DocumentNode<any, any>> = TDocumentNode extends DocumentNode<  infer TType,  any>  ? TType  : never;