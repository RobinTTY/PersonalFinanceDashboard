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
  DateTime: { input: Date; output: Date; }
  Decimal: { input: number; output: number; }
  URL: { input: unknown; output: unknown; }
  UUID: { input: unknown; output: unknown; }
};

export type AuthenticationRequest = {
  __typename: 'AuthenticationRequest';
  associatedAccounts: Array<BankAccount>;
  authenticationLink: Scalars['URL']['output'];
  createdAt: Scalars['DateTime']['output'];
  id: Maybe<Scalars['UUID']['output']>;
  status: AuthenticationStatus;
  thirdPartyId: Scalars['UUID']['output'];
};

export type AuthenticationRequestInput = {
  associatedAccounts: Array<BankAccountInput>;
  authenticationLink: Scalars['URL']['input'];
  createdAt: Scalars['DateTime']['input'];
  id?: InputMaybe<Scalars['UUID']['input']>;
  status: AuthenticationStatus;
  thirdPartyId: Scalars['UUID']['input'];
};

export enum AuthenticationStatus {
  Active = 'ACTIVE',
  Expired = 'EXPIRED',
  Failed = 'FAILED',
  Pending = 'PENDING',
  RequiresUserAction = 'REQUIRES_USER_ACTION',
  Unknown = 'UNKNOWN'
}

export type BankAccount = {
  __typename: 'BankAccount';
  accountType: Maybe<Scalars['String']['output']>;
  associatedAuthenticationRequests: Array<AuthenticationRequest>;
  associatedInstitution: Maybe<BankingInstitution>;
  balance: Maybe<Scalars['Decimal']['output']>;
  bban: Maybe<Scalars['String']['output']>;
  bic: Maybe<Scalars['String']['output']>;
  currency: Maybe<Scalars['String']['output']>;
  description: Maybe<Scalars['String']['output']>;
  iban: Maybe<Scalars['String']['output']>;
  id: Maybe<Scalars['UUID']['output']>;
  name: Maybe<Scalars['String']['output']>;
  ownerName: Maybe<Scalars['String']['output']>;
  thirdPartyId: Scalars['UUID']['output'];
  transactions: Array<Transaction>;
};

export type BankAccountInput = {
  accountType?: InputMaybe<Scalars['String']['input']>;
  associatedAuthenticationRequests: Array<AuthenticationRequestInput>;
  associatedInstitution?: InputMaybe<BankingInstitutionInput>;
  balance?: InputMaybe<Scalars['Decimal']['input']>;
  bban?: InputMaybe<Scalars['String']['input']>;
  bic?: InputMaybe<Scalars['String']['input']>;
  currency?: InputMaybe<Scalars['String']['input']>;
  description?: InputMaybe<Scalars['String']['input']>;
  iban?: InputMaybe<Scalars['String']['input']>;
  id?: InputMaybe<Scalars['UUID']['input']>;
  name?: InputMaybe<Scalars['String']['input']>;
  ownerName?: InputMaybe<Scalars['String']['input']>;
  thirdPartyId: Scalars['UUID']['input'];
  transactions: Array<TransactionInput>;
};

/** A connection to a list of items. */
export type BankAccountsConnection = {
  __typename: 'BankAccountsConnection';
  /** A list of edges. */
  edges: Maybe<Array<BankAccountsEdge>>;
  /** A flattened list of the nodes. */
  nodes: Maybe<Array<BankAccount>>;
  /** Information to aid in pagination. */
  pageInfo: PageInfo;
};

/** An edge in a connection. */
export type BankAccountsEdge = {
  __typename: 'BankAccountsEdge';
  /** A cursor for use in pagination. */
  cursor: Scalars['String']['output'];
  /** The item at the end of the edge. */
  node: BankAccount;
};

export type BankingInstitution = {
  __typename: 'BankingInstitution';
  bic: Scalars['String']['output'];
  countries: Array<Scalars['String']['output']>;
  id: Maybe<Scalars['UUID']['output']>;
  logoUri: Scalars['URL']['output'];
  name: Scalars['String']['output'];
  thirdPartyId: Scalars['String']['output'];
};

export type BankingInstitutionInput = {
  bic: Scalars['String']['input'];
  countries: Array<Scalars['String']['input']>;
  id?: InputMaybe<Scalars['UUID']['input']>;
  logoUri: Scalars['URL']['input'];
  name: Scalars['String']['input'];
  thirdPartyId: Scalars['String']['input'];
};

/** A connection to a list of items. */
export type BankingInstitutionsConnection = {
  __typename: 'BankingInstitutionsConnection';
  /** A list of edges. */
  edges: Maybe<Array<BankingInstitutionsEdge>>;
  /** A flattened list of the nodes. */
  nodes: Maybe<Array<BankingInstitution>>;
  /** Information to aid in pagination. */
  pageInfo: PageInfo;
};

/** An edge in a connection. */
export type BankingInstitutionsEdge = {
  __typename: 'BankingInstitutionsEdge';
  /** A cursor for use in pagination. */
  cursor: Scalars['String']['output'];
  /** The item at the end of the edge. */
  node: BankingInstitution;
};

export type CreateAuthenticationRequestInput = {
  /** The id of the institution which the authentication request should be created for. */
  institutionId: Scalars['String']['input'];
  /** The URI to redirect to after the authentication is completed. */
  redirectUri: Scalars['String']['input'];
};

export type CreateAuthenticationRequestPayload = {
  __typename: 'CreateAuthenticationRequestPayload';
  authenticationRequest: Maybe<AuthenticationRequest>;
};

export type CreateBankAccountInput = {
  /** The banking account to create. */
  bankAccount: BankAccountInput;
};

export type CreateBankAccountPayload = {
  __typename: 'CreateBankAccountPayload';
  bankAccount: Maybe<BankAccount>;
};

export type CreateBankingInstitutionInput = {
  /** The banking institution to create. */
  bankingInstitution: BankingInstitutionInput;
};

export type CreateBankingInstitutionPayload = {
  __typename: 'CreateBankingInstitutionPayload';
  bankingInstitution: Maybe<BankingInstitution>;
};

/** Payload required to create a transaction through the API. */
export type CreateTransactionInput = {
  /** The id of the account to which the transaction belongs. */
  accountId: Scalars['UUID']['input'];
  /** The amount being transacted. */
  amount: Scalars['Decimal']['input'];
  /** The category this transaction belongs to. */
  category: Scalars['String']['input'];
  /** The currency the amount is denominated in. */
  currency: Scalars['String']['input'];
  /** User created notes for this transaction. */
  notes: Scalars['String']['input'];
  /** The name of the party which is owed the money. */
  payee?: InputMaybe<Scalars['String']['input']>;
  /** The name of the party which owes the money. */
  payer?: InputMaybe<Scalars['String']['input']>;
  /** Date at which the transaction amount becomes available to the payee. */
  valueDate?: InputMaybe<Scalars['DateTime']['input']>;
};

export type CreateTransactionPayload = {
  __typename: 'CreateTransactionPayload';
  transaction: Maybe<Transaction>;
};

export type DeleteAuthenticationRequestInput = {
  /** The id of the authentication request to delete. */
  authenticationId: Scalars['UUID']['input'];
};

export type DeleteAuthenticationRequestPayload = {
  __typename: 'DeleteAuthenticationRequestPayload';
  deleteResponse: Maybe<DeleteResponse>;
};

export type DeleteBankAccountInput = {
  /** The id of the bank account to delete. */
  bankAccountId: Scalars['UUID']['input'];
};

export type DeleteBankAccountPayload = {
  __typename: 'DeleteBankAccountPayload';
  boolean: Maybe<Scalars['Boolean']['output']>;
};

export type DeleteBankingInstitutionInput = {
  /** The id of the banking institution to delete. */
  bankingInstitutionId: Scalars['UUID']['input'];
};

export type DeleteBankingInstitutionPayload = {
  __typename: 'DeleteBankingInstitutionPayload';
  boolean: Maybe<Scalars['Boolean']['output']>;
};

export type DeleteResponse = {
  __typename: 'DeleteResponse';
  /** The id of the deleted entity. */
  deletedId: Scalars['UUID']['output'];
};

export type DeleteTransactionInput = {
  /** The id of the transaction to delete. */
  transactionId: Scalars['UUID']['input'];
};

export type DeleteTransactionPayload = {
  __typename: 'DeleteTransactionPayload';
  response: Maybe<Response>;
};

export type Error = {
  message: Scalars['String']['output'];
};

/** AuthenticationRequest related mutation resolvers. */
export type Mutation = {
  __typename: 'Mutation';
  /** Create a new authentication request for an institution. */
  createAuthenticationRequest: CreateAuthenticationRequestPayload;
  /** Create a new banking account. */
  createBankAccount: CreateBankAccountPayload;
  /** Create a new banking institution. */
  createBankingInstitution: CreateBankingInstitutionPayload;
  /** Create a new transaction. */
  createTransaction: CreateTransactionPayload;
  /** Delete an existing authentication request. */
  deleteAuthenticationRequest: DeleteAuthenticationRequestPayload;
  /** Delete an existing bank account. */
  deleteBankAccount: DeleteBankAccountPayload;
  /** Delete an existing banking institution. */
  deleteBankingInstitution: DeleteBankingInstitutionPayload;
  /** Delete an existing transaction. */
  deleteTransaction: DeleteTransactionPayload;
  /**
   * Synchronizes authentication request data with third-party data providers.
   *
   *
   * **Returns:**
   * A boolean value indicating whether the synchronization was successful.
   */
  synchronizeAuthenticationRequestData: SynchronizeAuthenticationRequestDataPayload;
  /**
   * Synchronizes bank account data with third-party data providers.
   *
   *
   * **Returns:**
   * A boolean value indicating whether the synchronization was successful.
   */
  synchronizeBankAccountData: SynchronizeBankAccountDataPayload;
  /**
   * Synchronizes banking institution data with third-party data providers.
   *
   *
   * **Returns:**
   * A boolean value indicating whether the synchronization was successful.
   */
  synchronizeBankingInstitutionData: SynchronizeBankingInstitutionDataPayload;
  /**
   * Synchronizes transaction data with third-party data providers.
   *
   *
   * **Returns:**
   * A boolean value indicating whether the synchronization was successful.
   */
  synchronizeTransactionData: SynchronizeTransactionDataPayload;
  /** Update an existing banking account. */
  updateBankAccount: UpdateBankAccountPayload;
  /** Update an existing banking institution. */
  updateBankingInstitution: UpdateBankingInstitutionPayload;
  /** Update an existing transaction. */
  updateTransaction: UpdateTransactionPayload;
};


/** AuthenticationRequest related mutation resolvers. */
export type MutationCreateAuthenticationRequestArgs = {
  input: CreateAuthenticationRequestInput;
};


/** AuthenticationRequest related mutation resolvers. */
export type MutationCreateBankAccountArgs = {
  input: CreateBankAccountInput;
};


/** AuthenticationRequest related mutation resolvers. */
export type MutationCreateBankingInstitutionArgs = {
  input: CreateBankingInstitutionInput;
};


/** AuthenticationRequest related mutation resolvers. */
export type MutationCreateTransactionArgs = {
  input: CreateTransactionInput;
};


/** AuthenticationRequest related mutation resolvers. */
export type MutationDeleteAuthenticationRequestArgs = {
  input: DeleteAuthenticationRequestInput;
};


/** AuthenticationRequest related mutation resolvers. */
export type MutationDeleteBankAccountArgs = {
  input: DeleteBankAccountInput;
};


/** AuthenticationRequest related mutation resolvers. */
export type MutationDeleteBankingInstitutionArgs = {
  input: DeleteBankingInstitutionInput;
};


/** AuthenticationRequest related mutation resolvers. */
export type MutationDeleteTransactionArgs = {
  input: DeleteTransactionInput;
};


/** AuthenticationRequest related mutation resolvers. */
export type MutationUpdateBankAccountArgs = {
  input: UpdateBankAccountInput;
};


/** AuthenticationRequest related mutation resolvers. */
export type MutationUpdateBankingInstitutionArgs = {
  input: UpdateBankingInstitutionInput;
};


/** AuthenticationRequest related mutation resolvers. */
export type MutationUpdateTransactionArgs = {
  input: UpdateTransactionInput;
};

/** Information about pagination in a connection. */
export type PageInfo = {
  __typename: 'PageInfo';
  /** When paginating forwards, the cursor to continue. */
  endCursor: Maybe<Scalars['String']['output']>;
  /** Indicates whether more edges exist following the set defined by the clients arguments. */
  hasNextPage: Scalars['Boolean']['output'];
  /** Indicates whether more edges exist prior the set defined by the clients arguments. */
  hasPreviousPage: Scalars['Boolean']['output'];
  /** When paginating backwards, the cursor to continue. */
  startCursor: Maybe<Scalars['String']['output']>;
};

/** AuthenticationRequest related query resolvers. */
export type Query = {
  __typename: 'Query';
  /** Look up authentication requests by their id. */
  authenticationRequest: Maybe<AuthenticationRequest>;
  /** Look up authentication requests. */
  authenticationRequests: Array<AuthenticationRequest>;
  /** Look up an account by its id. */
  bankAccount: Maybe<BankAccount>;
  /** Look up accounts. */
  bankAccounts: Maybe<BankAccountsConnection>;
  /** Look up banking institutions by their id. */
  bankingInstitution: Maybe<BankingInstitution>;
  /** Look up banking institutions. */
  bankingInstitutions: Maybe<BankingInstitutionsConnection>;
  /** Look up transactions. */
  transactions: Array<Transaction>;
  /** Look up transactions of an account. */
  transactionsByAccountId: Maybe<TransactionsByAccountIdConnection>;
};


/** AuthenticationRequest related query resolvers. */
export type QueryAuthenticationRequestArgs = {
  authenticationId: Scalars['UUID']['input'];
};


/** AuthenticationRequest related query resolvers. */
export type QueryBankAccountArgs = {
  accountId: Scalars['UUID']['input'];
};


/** AuthenticationRequest related query resolvers. */
export type QueryBankAccountsArgs = {
  after?: InputMaybe<Scalars['String']['input']>;
  before?: InputMaybe<Scalars['String']['input']>;
  first?: InputMaybe<Scalars['Int']['input']>;
  last?: InputMaybe<Scalars['Int']['input']>;
};


/** AuthenticationRequest related query resolvers. */
export type QueryBankingInstitutionArgs = {
  institutionId: Scalars['UUID']['input'];
};


/** AuthenticationRequest related query resolvers. */
export type QueryBankingInstitutionsArgs = {
  after?: InputMaybe<Scalars['String']['input']>;
  before?: InputMaybe<Scalars['String']['input']>;
  countryCode?: InputMaybe<Scalars['String']['input']>;
  first?: InputMaybe<Scalars['Int']['input']>;
  last?: InputMaybe<Scalars['Int']['input']>;
};


/** AuthenticationRequest related query resolvers. */
export type QueryTransactionsByAccountIdArgs = {
  accountId: Scalars['UUID']['input'];
  after?: InputMaybe<Scalars['String']['input']>;
  before?: InputMaybe<Scalars['String']['input']>;
  first?: InputMaybe<Scalars['Int']['input']>;
  last?: InputMaybe<Scalars['Int']['input']>;
};

/** TODO */
export type Response = {
  __typename: 'Response';
  /** TODO */
  id: Scalars['UUID']['output'];
};

export type SynchronizeAuthenticationRequestDataPayload = {
  __typename: 'SynchronizeAuthenticationRequestDataPayload';
  boolean: Maybe<Scalars['Boolean']['output']>;
};

export type SynchronizeBankAccountDataPayload = {
  __typename: 'SynchronizeBankAccountDataPayload';
  boolean: Maybe<Scalars['Boolean']['output']>;
};

export type SynchronizeBankingInstitutionDataPayload = {
  __typename: 'SynchronizeBankingInstitutionDataPayload';
  boolean: Maybe<Scalars['Boolean']['output']>;
};

export type SynchronizeTransactionDataPayload = {
  __typename: 'SynchronizeTransactionDataPayload';
  boolean: Maybe<Scalars['Boolean']['output']>;
};

export type Tag = {
  __typename: 'Tag';
  color: Scalars['String']['output'];
  description: Scalars['String']['output'];
  id: Scalars['UUID']['output'];
  name: Scalars['String']['output'];
  transactions: Array<Transaction>;
};

export type TagInput = {
  color: Scalars['String']['input'];
  description: Scalars['String']['input'];
  id: Scalars['UUID']['input'];
  name: Scalars['String']['input'];
  transactions: Array<TransactionInput>;
};

export type Transaction = {
  __typename: 'Transaction';
  amount: Scalars['Decimal']['output'];
  bankAccount: Maybe<BankAccount>;
  bankTransactionId: Maybe<Scalars['String']['output']>;
  category: Scalars['String']['output'];
  currency: Scalars['String']['output'];
  id: Maybe<Scalars['UUID']['output']>;
  notes: Scalars['String']['output'];
  payee: Maybe<Scalars['String']['output']>;
  payer: Maybe<Scalars['String']['output']>;
  tags: Array<Tag>;
  thirdPartyId: Scalars['UUID']['output'];
  valueDate: Maybe<Scalars['DateTime']['output']>;
};

export type TransactionInput = {
  amount: Scalars['Decimal']['input'];
  bankAccount?: InputMaybe<BankAccountInput>;
  bankTransactionId?: InputMaybe<Scalars['String']['input']>;
  category: Scalars['String']['input'];
  currency: Scalars['String']['input'];
  id?: InputMaybe<Scalars['UUID']['input']>;
  notes: Scalars['String']['input'];
  payee?: InputMaybe<Scalars['String']['input']>;
  payer?: InputMaybe<Scalars['String']['input']>;
  tags: Array<TagInput>;
  thirdPartyId: Scalars['UUID']['input'];
  valueDate?: InputMaybe<Scalars['DateTime']['input']>;
};

/** A connection to a list of items. */
export type TransactionsByAccountIdConnection = {
  __typename: 'TransactionsByAccountIdConnection';
  /** A list of edges. */
  edges: Maybe<Array<TransactionsByAccountIdEdge>>;
  /** A flattened list of the nodes. */
  nodes: Maybe<Array<Transaction>>;
  /** Information to aid in pagination. */
  pageInfo: PageInfo;
};

/** An edge in a connection. */
export type TransactionsByAccountIdEdge = {
  __typename: 'TransactionsByAccountIdEdge';
  /** A cursor for use in pagination. */
  cursor: Scalars['String']['output'];
  /** The item at the end of the edge. */
  node: Transaction;
};

export type UpdateBankAccountInput = {
  /** The banking account to update. */
  bankAccount: BankAccountInput;
};

export type UpdateBankAccountPayload = {
  __typename: 'UpdateBankAccountPayload';
  bankAccount: Maybe<BankAccount>;
};

export type UpdateBankingInstitutionInput = {
  /** The banking institution to update. */
  bankingInstitution: BankingInstitutionInput;
};

export type UpdateBankingInstitutionPayload = {
  __typename: 'UpdateBankingInstitutionPayload';
  bankingInstitution: Maybe<BankingInstitution>;
};

export type UpdateTransactionInput = {
  /** The transaction to update. */
  transaction: TransactionInput;
};

export type UpdateTransactionPayload = {
  __typename: 'UpdateTransactionPayload';
  transaction: Maybe<Transaction>;
};

export type AuthRequestWithAccountsFragment = { __typename: 'AuthenticationRequest', id: unknown | null, thirdPartyId: unknown, status: AuthenticationStatus, authenticationLink: unknown, createdAt: Date, associatedAccounts: Array<{ __typename: 'BankAccount', id: unknown | null, thirdPartyId: unknown, iban: string | null, name: string | null, description: string | null, accountType: string | null, balance: number | null, currency: string | null, ownerName: string | null, associatedInstitution: { __typename: 'BankingInstitution', name: string, bic: string, logoUri: unknown } | null }> };

export type CreateAuthenticationRequestMutationVariables = Exact<{
  institutionId: Scalars['String']['input'];
  redirectUri: Scalars['String']['input'];
}>;


export type CreateAuthenticationRequestMutation = { createAuthenticationRequest: { __typename: 'CreateAuthenticationRequestPayload', authenticationRequest: { __typename: 'AuthenticationRequest', id: unknown | null, thirdPartyId: unknown, status: AuthenticationStatus, authenticationLink: unknown, associatedAccounts: Array<{ __typename: 'BankAccount', id: unknown | null }> } | null } };

export type DeleteAuthenticationRequestMutationVariables = Exact<{
  authenticationId: Scalars['UUID']['input'];
}>;


export type DeleteAuthenticationRequestMutation = { deleteAuthenticationRequest: { __typename: 'DeleteAuthenticationRequestPayload', deleteResponse: { __typename: 'DeleteResponse', deletedId: unknown } | null } };

export type GetAuthRequestWithAccountsQueryVariables = Exact<{
  authenticationId: Scalars['UUID']['input'];
}>;


export type GetAuthRequestWithAccountsQuery = { authenticationRequest: { __typename: 'AuthenticationRequest', id: unknown | null, thirdPartyId: unknown, status: AuthenticationStatus, authenticationLink: unknown, createdAt: Date, associatedAccounts: Array<{ __typename: 'BankAccount', id: unknown | null, thirdPartyId: unknown, iban: string | null, name: string | null, description: string | null, accountType: string | null, balance: number | null, currency: string | null, ownerName: string | null, associatedInstitution: { __typename: 'BankingInstitution', name: string, bic: string, logoUri: unknown } | null }> } | null };

export type GetAuthRequestsWithAccountsQueryVariables = Exact<{ [key: string]: never; }>;


export type GetAuthRequestsWithAccountsQuery = { authenticationRequests: Array<{ __typename: 'AuthenticationRequest', id: unknown | null, thirdPartyId: unknown, status: AuthenticationStatus, authenticationLink: unknown, createdAt: Date, associatedAccounts: Array<{ __typename: 'BankAccount', id: unknown | null, thirdPartyId: unknown, iban: string | null, name: string | null, description: string | null, accountType: string | null, balance: number | null, currency: string | null, ownerName: string | null, associatedInstitution: { __typename: 'BankingInstitution', name: string, bic: string, logoUri: unknown } | null }> }> };

export type GetAuthenticationRequestQueryVariables = Exact<{
  authenticationId: Scalars['UUID']['input'];
}>;


export type GetAuthenticationRequestQuery = { authenticationRequest: { __typename: 'AuthenticationRequest', id: unknown | null, thirdPartyId: unknown, status: AuthenticationStatus, authenticationLink: unknown, associatedAccounts: Array<{ __typename: 'BankAccount', id: unknown | null }> } | null };

export type GetBankingInstitutionsQueryVariables = Exact<{
  countryCode?: InputMaybe<Scalars['String']['input']>;
  first?: InputMaybe<Scalars['Int']['input']>;
}>;


export type GetBankingInstitutionsQuery = { bankingInstitutions: { __typename: 'BankingInstitutionsConnection', pageInfo: { __typename: 'PageInfo', hasNextPage: boolean, hasPreviousPage: boolean, startCursor: string | null, endCursor: string | null }, edges: Array<{ __typename: 'BankingInstitutionsEdge', node: { __typename: 'BankingInstitution', id: unknown | null, thirdPartyId: string, name: string, bic: string, logoUri: unknown } }> | null } | null };
