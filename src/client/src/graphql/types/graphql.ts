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
  /** The `Decimal` scalar type represents a decimal floating-point number. */
  Decimal: { input: number; output: number; }
  URL: { input: unknown; output: unknown; }
  UUID: { input: unknown; output: unknown; }
};

export type AuthenticationRequest = {
  __typename: 'AuthenticationRequest';
  /** When the access to the accounts granted by this request expires. */
  accessExpiresAt: Scalars['DateTime']['output'];
  accessValidForDays: Maybe<Scalars['Int']['output']>;
  associatedAccounts: Array<BankAccount>;
  authenticationLink: Scalars['URL']['output'];
  createdAt: Scalars['DateTime']['output'];
  externalId: Maybe<Scalars['String']['output']>;
  id: Maybe<Scalars['UUID']['output']>;
  institution: Maybe<BankingInstitution>;
  source: DataSource;
  status: AuthenticationStatus;
};

export type AuthenticationRequestFilterInput = {
  accessValidForDays?: InputMaybe<IntOperationFilterInput>;
  and?: InputMaybe<Array<AuthenticationRequestFilterInput>>;
  associatedAccounts?: InputMaybe<ListFilterInputTypeOfBankAccountFilterInput>;
  authenticationLink?: InputMaybe<UrlOperationFilterInput>;
  createdAt?: InputMaybe<DateTimeOperationFilterInput>;
  externalId?: InputMaybe<StringOperationFilterInput>;
  id?: InputMaybe<UuidOperationFilterInput>;
  institution?: InputMaybe<BankingInstitutionFilterInput>;
  or?: InputMaybe<Array<AuthenticationRequestFilterInput>>;
  source?: InputMaybe<DataSourceOperationFilterInput>;
  status?: InputMaybe<AuthenticationStatusOperationFilterInput>;
};

export enum AuthenticationStatus {
  Active = 'ACTIVE',
  Expired = 'EXPIRED',
  Failed = 'FAILED',
  Pending = 'PENDING',
  RequiresUserAction = 'REQUIRES_USER_ACTION',
  Unknown = 'UNKNOWN'
}

export type AuthenticationStatusOperationFilterInput = {
  eq?: InputMaybe<AuthenticationStatus>;
  in?: InputMaybe<Array<AuthenticationStatus>>;
  neq?: InputMaybe<AuthenticationStatus>;
  nin?: InputMaybe<Array<AuthenticationStatus>>;
};

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
  externalId: Maybe<Scalars['String']['output']>;
  iban: Maybe<Scalars['String']['output']>;
  id: Maybe<Scalars['UUID']['output']>;
  includeInAnalytics: Scalars['Boolean']['output'];
  maskedPan: Maybe<Scalars['String']['output']>;
  name: Maybe<Scalars['String']['output']>;
  ownerName: Maybe<Scalars['String']['output']>;
  providerDescription: Maybe<Scalars['String']['output']>;
  providerName: Maybe<Scalars['String']['output']>;
  source: DataSource;
  /** How up to date the account is with the data source it originates from. */
  syncStatus: SyncStatus;
  transactions: Array<Transaction>;
};

export type BankAccountFilterInput = {
  accountType?: InputMaybe<StringOperationFilterInput>;
  and?: InputMaybe<Array<BankAccountFilterInput>>;
  associatedAuthenticationRequests?: InputMaybe<ListFilterInputTypeOfAuthenticationRequestFilterInput>;
  associatedInstitution?: InputMaybe<BankingInstitutionFilterInput>;
  balance?: InputMaybe<DecimalOperationFilterInput>;
  bban?: InputMaybe<StringOperationFilterInput>;
  bic?: InputMaybe<StringOperationFilterInput>;
  currency?: InputMaybe<StringOperationFilterInput>;
  description?: InputMaybe<StringOperationFilterInput>;
  externalId?: InputMaybe<StringOperationFilterInput>;
  iban?: InputMaybe<StringOperationFilterInput>;
  id?: InputMaybe<UuidOperationFilterInput>;
  includeInAnalytics?: InputMaybe<BooleanOperationFilterInput>;
  maskedPan?: InputMaybe<StringOperationFilterInput>;
  name?: InputMaybe<StringOperationFilterInput>;
  or?: InputMaybe<Array<BankAccountFilterInput>>;
  ownerName?: InputMaybe<StringOperationFilterInput>;
  providerDescription?: InputMaybe<StringOperationFilterInput>;
  providerName?: InputMaybe<StringOperationFilterInput>;
  source?: InputMaybe<DataSourceOperationFilterInput>;
  transactions?: InputMaybe<ListFilterInputTypeOfTransactionFilterInput>;
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
  externalId: Maybe<Scalars['String']['output']>;
  id: Maybe<Scalars['UUID']['output']>;
  logoUri: Scalars['URL']['output'];
  name: Scalars['String']['output'];
  source: DataSource;
};

export type BankingInstitutionFilterInput = {
  and?: InputMaybe<Array<BankingInstitutionFilterInput>>;
  bic?: InputMaybe<StringOperationFilterInput>;
  countries?: InputMaybe<ListStringOperationFilterInput>;
  externalId?: InputMaybe<StringOperationFilterInput>;
  id?: InputMaybe<UuidOperationFilterInput>;
  logoUri?: InputMaybe<UrlOperationFilterInput>;
  name?: InputMaybe<StringOperationFilterInput>;
  or?: InputMaybe<Array<BankingInstitutionFilterInput>>;
  source?: InputMaybe<DataSourceOperationFilterInput>;
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

export type BooleanOperationFilterInput = {
  eq?: InputMaybe<Scalars['Boolean']['input']>;
  neq?: InputMaybe<Scalars['Boolean']['input']>;
};

export type CreateAuthenticationRequestInput = {
  /** The id of the institution which the authentication request should be created for. */
  institutionId: Scalars['UUID']['input'];
  /** The URI to redirect to after the authentication is completed. */
  redirectUri: Scalars['String']['input'];
};

export type CreateAuthenticationRequestPayload = {
  __typename: 'CreateAuthenticationRequestPayload';
  authenticationRequest: Maybe<AuthenticationRequest>;
};

/** Payload required to create a bank account through the API. */
export type CreateBankAccountInput = {
  /** Specifies the nature, or use, of the account. */
  accountType?: InputMaybe<Scalars['String']['input']>;
  /** The current balance of the account. */
  balance?: InputMaybe<Scalars['Decimal']['input']>;
  /**
   * Basic Bank Account Number represents a country-specific bank account number.
   * This data element is used for payment accounts which have no IBAN.
   */
  bban?: InputMaybe<Scalars['String']['input']>;
  /** The BIC (Business Identifier Code) associated with the account. */
  bic?: InputMaybe<Scalars['String']['input']>;
  /** The currency this account is denominated in. */
  currency?: InputMaybe<Scalars['String']['input']>;
  /** A description of this account. */
  description?: InputMaybe<Scalars['String']['input']>;
  /** The IBAN of the bank account. */
  iban?: InputMaybe<Scalars['String']['input']>;
  /** The name of the account. */
  name: Scalars['String']['input'];
  /** Name of the legal account owner. If there is more than one owner, then two names might be noted here. */
  ownerName?: InputMaybe<Scalars['String']['input']>;
};

export type CreateBankAccountPayload = {
  __typename: 'CreateBankAccountPayload';
  bankAccount: Maybe<BankAccount>;
};

/** Payload required to create a banking institution through the API. */
export type CreateBankingInstitutionInput = {
  /** The Business Identifier Code (BIC) of the institution. */
  bic: Scalars['String']['input'];
  /** The countries the institution operates in. */
  countries: Array<Scalars['String']['input']>;
  /** A Uri for the logo of the institution. */
  logoUri: Scalars['URL']['input'];
  /** The name of the institution. */
  name: Scalars['String']['input'];
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
  category?: InputMaybe<Scalars['String']['input']>;
  /** The currency the amount is denominated in. */
  currency: Scalars['String']['input'];
  /** What the transaction is for. */
  description?: InputMaybe<Scalars['String']['input']>;
  /** User created notes for this transaction. */
  notes?: InputMaybe<Scalars['String']['input']>;
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

export enum DataSource {
  GoCardless = 'GO_CARDLESS',
  Manual = 'MANUAL'
}

export type DataSourceOperationFilterInput = {
  eq?: InputMaybe<DataSource>;
  in?: InputMaybe<Array<DataSource>>;
  neq?: InputMaybe<DataSource>;
  nin?: InputMaybe<Array<DataSource>>;
};

/** A synchronization that retrieved data from a data provider. */
export type DataSynchronization = {
  __typename: 'DataSynchronization';
  /** When the synchronization completed. */
  completedAt: Scalars['DateTime']['output'];
};

export type DateTimeOperationFilterInput = {
  eq?: InputMaybe<Scalars['DateTime']['input']>;
  gt?: InputMaybe<Scalars['DateTime']['input']>;
  gte?: InputMaybe<Scalars['DateTime']['input']>;
  in?: InputMaybe<Array<InputMaybe<Scalars['DateTime']['input']>>>;
  lt?: InputMaybe<Scalars['DateTime']['input']>;
  lte?: InputMaybe<Scalars['DateTime']['input']>;
  neq?: InputMaybe<Scalars['DateTime']['input']>;
  ngt?: InputMaybe<Scalars['DateTime']['input']>;
  ngte?: InputMaybe<Scalars['DateTime']['input']>;
  nin?: InputMaybe<Array<InputMaybe<Scalars['DateTime']['input']>>>;
  nlt?: InputMaybe<Scalars['DateTime']['input']>;
  nlte?: InputMaybe<Scalars['DateTime']['input']>;
};

export type DecimalOperationFilterInput = {
  eq?: InputMaybe<Scalars['Decimal']['input']>;
  gt?: InputMaybe<Scalars['Decimal']['input']>;
  gte?: InputMaybe<Scalars['Decimal']['input']>;
  in?: InputMaybe<Array<InputMaybe<Scalars['Decimal']['input']>>>;
  lt?: InputMaybe<Scalars['Decimal']['input']>;
  lte?: InputMaybe<Scalars['Decimal']['input']>;
  neq?: InputMaybe<Scalars['Decimal']['input']>;
  ngt?: InputMaybe<Scalars['Decimal']['input']>;
  ngte?: InputMaybe<Scalars['Decimal']['input']>;
  nin?: InputMaybe<Array<InputMaybe<Scalars['Decimal']['input']>>>;
  nlt?: InputMaybe<Scalars['Decimal']['input']>;
  nlte?: InputMaybe<Scalars['Decimal']['input']>;
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
  deleteResponse: Maybe<DeleteResponse>;
};

export type DeleteBankingInstitutionInput = {
  /** The id of the banking institution to delete. */
  bankingInstitutionId: Scalars['UUID']['input'];
};

export type DeleteBankingInstitutionPayload = {
  __typename: 'DeleteBankingInstitutionPayload';
  deleteResponse: Maybe<DeleteResponse>;
};

/** The result of a successful delete mutation. */
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
  deleteResponse: Maybe<DeleteResponse>;
};

export type Error = {
  message: Scalars['String']['output'];
};

export type IntOperationFilterInput = {
  eq?: InputMaybe<Scalars['Int']['input']>;
  gt?: InputMaybe<Scalars['Int']['input']>;
  gte?: InputMaybe<Scalars['Int']['input']>;
  in?: InputMaybe<Array<InputMaybe<Scalars['Int']['input']>>>;
  lt?: InputMaybe<Scalars['Int']['input']>;
  lte?: InputMaybe<Scalars['Int']['input']>;
  neq?: InputMaybe<Scalars['Int']['input']>;
  ngt?: InputMaybe<Scalars['Int']['input']>;
  ngte?: InputMaybe<Scalars['Int']['input']>;
  nin?: InputMaybe<Array<InputMaybe<Scalars['Int']['input']>>>;
  nlt?: InputMaybe<Scalars['Int']['input']>;
  nlte?: InputMaybe<Scalars['Int']['input']>;
};

export type ListFilterInputTypeOfAuthenticationRequestFilterInput = {
  all?: InputMaybe<AuthenticationRequestFilterInput>;
  any?: InputMaybe<Scalars['Boolean']['input']>;
  none?: InputMaybe<AuthenticationRequestFilterInput>;
  some?: InputMaybe<AuthenticationRequestFilterInput>;
};

export type ListFilterInputTypeOfBankAccountFilterInput = {
  all?: InputMaybe<BankAccountFilterInput>;
  any?: InputMaybe<Scalars['Boolean']['input']>;
  none?: InputMaybe<BankAccountFilterInput>;
  some?: InputMaybe<BankAccountFilterInput>;
};

export type ListFilterInputTypeOfTransactionFilterInput = {
  all?: InputMaybe<TransactionFilterInput>;
  any?: InputMaybe<Scalars['Boolean']['input']>;
  none?: InputMaybe<TransactionFilterInput>;
  some?: InputMaybe<TransactionFilterInput>;
};

export type ListStringOperationFilterInput = {
  all?: InputMaybe<StringOperationFilterInput>;
  any?: InputMaybe<Scalars['Boolean']['input']>;
  none?: InputMaybe<StringOperationFilterInput>;
  some?: InputMaybe<StringOperationFilterInput>;
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
  /** Sets whether a bank account is included in analytics such as graphs and account views. */
  setBankAccountIncludeInAnalytics: SetBankAccountIncludeInAnalyticsPayload;
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
  /** Update the name and description of an existing banking account. */
  updateBankAccount: UpdateBankAccountPayload;
  /** Update an existing banking institution. */
  updateBankingInstitution: UpdateBankingInstitutionPayload;
  /** Update the category and notes of an existing transaction. */
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
export type MutationSetBankAccountIncludeInAnalyticsArgs = {
  input: SetBankAccountIncludeInAnalyticsInput;
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
  transactions: Maybe<TransactionsConnection>;
  /** Look up transactions of an account. */
  transactionsByAccountId: Maybe<TransactionsByAccountIdConnection>;
};


/** AuthenticationRequest related query resolvers. */
export type QueryAuthenticationRequestArgs = {
  authenticationId: Scalars['UUID']['input'];
  refresh?: Scalars['Boolean']['input'];
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
  where?: InputMaybe<BankAccountFilterInput>;
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
export type QueryTransactionsArgs = {
  after?: InputMaybe<Scalars['String']['input']>;
  before?: InputMaybe<Scalars['String']['input']>;
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

export type SetBankAccountIncludeInAnalyticsInput = {
  /** The id of the bank account to update. */
  bankAccountId: Scalars['UUID']['input'];
  /** Whether the account should be included in analytics. */
  includeInAnalytics: Scalars['Boolean']['input'];
};

export type SetBankAccountIncludeInAnalyticsPayload = {
  __typename: 'SetBankAccountIncludeInAnalyticsPayload';
  bankAccount: Maybe<BankAccount>;
};

export type StringOperationFilterInput = {
  and?: InputMaybe<Array<StringOperationFilterInput>>;
  contains?: InputMaybe<Scalars['String']['input']>;
  endsWith?: InputMaybe<Scalars['String']['input']>;
  eq?: InputMaybe<Scalars['String']['input']>;
  in?: InputMaybe<Array<InputMaybe<Scalars['String']['input']>>>;
  ncontains?: InputMaybe<Scalars['String']['input']>;
  nendsWith?: InputMaybe<Scalars['String']['input']>;
  neq?: InputMaybe<Scalars['String']['input']>;
  nin?: InputMaybe<Array<InputMaybe<Scalars['String']['input']>>>;
  nstartsWith?: InputMaybe<Scalars['String']['input']>;
  or?: InputMaybe<Array<StringOperationFilterInput>>;
  startsWith?: InputMaybe<Scalars['String']['input']>;
};

/** Subscriptions to the synchronization of third-party data. */
export type Subscription = {
  __typename: 'Subscription';
  /**
   * Is sent whenever a synchronization retrieved data from a data provider, successfully or not, so the data and
   * its sync status can be refetched.
   */
  dataSynchronized: DataSynchronization;
};

export enum SyncState {
  NeedsReauthentication = 'NEEDS_REAUTHENTICATION',
  NotSynced = 'NOT_SYNCED',
  Retrying = 'RETRYING',
  UpToDate = 'UP_TO_DATE'
}

export type SyncStatus = {
  __typename: 'SyncStatus';
  lastError: Maybe<Scalars['String']['output']>;
  lastSuccessfulSync: Maybe<Scalars['DateTime']['output']>;
  nextRetry: Maybe<Scalars['DateTime']['output']>;
  state: SyncState;
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

export type Transaction = {
  __typename: 'Transaction';
  amount: Scalars['Decimal']['output'];
  bankAccount: Maybe<BankAccount>;
  bankTransactionId: Maybe<Scalars['String']['output']>;
  bookingDate: Maybe<Scalars['DateTime']['output']>;
  category: Maybe<Scalars['String']['output']>;
  currency: Scalars['String']['output'];
  description: Maybe<Scalars['String']['output']>;
  externalId: Maybe<Scalars['String']['output']>;
  id: Maybe<Scalars['UUID']['output']>;
  notes: Maybe<Scalars['String']['output']>;
  payee: Maybe<Scalars['String']['output']>;
  payer: Maybe<Scalars['String']['output']>;
  source: DataSource;
  status: TransactionStatus;
  valueDate: Maybe<Scalars['DateTime']['output']>;
};

export type TransactionFilterInput = {
  amount?: InputMaybe<DecimalOperationFilterInput>;
  and?: InputMaybe<Array<TransactionFilterInput>>;
  bankAccount?: InputMaybe<BankAccountFilterInput>;
  bankTransactionId?: InputMaybe<StringOperationFilterInput>;
  bookingDate?: InputMaybe<DateTimeOperationFilterInput>;
  category?: InputMaybe<StringOperationFilterInput>;
  currency?: InputMaybe<StringOperationFilterInput>;
  description?: InputMaybe<StringOperationFilterInput>;
  externalId?: InputMaybe<StringOperationFilterInput>;
  id?: InputMaybe<UuidOperationFilterInput>;
  notes?: InputMaybe<StringOperationFilterInput>;
  or?: InputMaybe<Array<TransactionFilterInput>>;
  payee?: InputMaybe<StringOperationFilterInput>;
  payer?: InputMaybe<StringOperationFilterInput>;
  source?: InputMaybe<DataSourceOperationFilterInput>;
  status?: InputMaybe<TransactionStatusOperationFilterInput>;
  valueDate?: InputMaybe<DateTimeOperationFilterInput>;
};

export enum TransactionStatus {
  Booked = 'BOOKED',
  Pending = 'PENDING'
}

export type TransactionStatusOperationFilterInput = {
  eq?: InputMaybe<TransactionStatus>;
  in?: InputMaybe<Array<TransactionStatus>>;
  neq?: InputMaybe<TransactionStatus>;
  nin?: InputMaybe<Array<TransactionStatus>>;
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

/** A connection to a list of items. */
export type TransactionsConnection = {
  __typename: 'TransactionsConnection';
  /** A list of edges. */
  edges: Maybe<Array<TransactionsEdge>>;
  /** A flattened list of the nodes. */
  nodes: Maybe<Array<Transaction>>;
  /** Information to aid in pagination. */
  pageInfo: PageInfo;
};

/** An edge in a connection. */
export type TransactionsEdge = {
  __typename: 'TransactionsEdge';
  /** A cursor for use in pagination. */
  cursor: Scalars['String']['output'];
  /** The item at the end of the edge. */
  node: Transaction;
};

/** Payload required to update a bank account through the API. */
export type UpdateBankAccountInput = {
  /** A description of this account. If empty, a synchronized account shows its bank's description again. */
  description?: InputMaybe<Scalars['String']['input']>;
  /** The id of the bank account to update. */
  id: Scalars['UUID']['input'];
  /** The name of the account. If empty, a synchronized account shows its bank's name again. */
  name: Scalars['String']['input'];
};

export type UpdateBankAccountPayload = {
  __typename: 'UpdateBankAccountPayload';
  bankAccount: Maybe<BankAccount>;
};

/** Payload required to update a banking institution through the API. */
export type UpdateBankingInstitutionInput = {
  /** The Business Identifier Code (BIC) of the institution. */
  bic: Scalars['String']['input'];
  /** The countries the institution operates in. */
  countries: Array<Scalars['String']['input']>;
  /** The id of the banking institution to update. */
  id: Scalars['UUID']['input'];
  /** A Uri for the logo of the institution. */
  logoUri: Scalars['URL']['input'];
  /** The name of the institution. */
  name: Scalars['String']['input'];
};

export type UpdateBankingInstitutionPayload = {
  __typename: 'UpdateBankingInstitutionPayload';
  bankingInstitution: Maybe<BankingInstitution>;
};

/** Payload required to update a transaction through the API. */
export type UpdateTransactionInput = {
  /** The category this transaction belongs to, empty or omitted to remove it. */
  category?: InputMaybe<Scalars['String']['input']>;
  /** The id of the transaction to update. */
  id: Scalars['UUID']['input'];
  /** User created notes for this transaction, empty or omitted to remove them. */
  notes?: InputMaybe<Scalars['String']['input']>;
};

export type UpdateTransactionPayload = {
  __typename: 'UpdateTransactionPayload';
  transaction: Maybe<Transaction>;
};

export type UrlOperationFilterInput = {
  eq?: InputMaybe<Scalars['URL']['input']>;
  gt?: InputMaybe<Scalars['URL']['input']>;
  gte?: InputMaybe<Scalars['URL']['input']>;
  in?: InputMaybe<Array<InputMaybe<Scalars['URL']['input']>>>;
  lt?: InputMaybe<Scalars['URL']['input']>;
  lte?: InputMaybe<Scalars['URL']['input']>;
  neq?: InputMaybe<Scalars['URL']['input']>;
  ngt?: InputMaybe<Scalars['URL']['input']>;
  ngte?: InputMaybe<Scalars['URL']['input']>;
  nin?: InputMaybe<Array<InputMaybe<Scalars['URL']['input']>>>;
  nlt?: InputMaybe<Scalars['URL']['input']>;
  nlte?: InputMaybe<Scalars['URL']['input']>;
};

export type UuidOperationFilterInput = {
  eq?: InputMaybe<Scalars['UUID']['input']>;
  gt?: InputMaybe<Scalars['UUID']['input']>;
  gte?: InputMaybe<Scalars['UUID']['input']>;
  in?: InputMaybe<Array<InputMaybe<Scalars['UUID']['input']>>>;
  lt?: InputMaybe<Scalars['UUID']['input']>;
  lte?: InputMaybe<Scalars['UUID']['input']>;
  neq?: InputMaybe<Scalars['UUID']['input']>;
  ngt?: InputMaybe<Scalars['UUID']['input']>;
  ngte?: InputMaybe<Scalars['UUID']['input']>;
  nin?: InputMaybe<Array<InputMaybe<Scalars['UUID']['input']>>>;
  nlt?: InputMaybe<Scalars['UUID']['input']>;
  nlte?: InputMaybe<Scalars['UUID']['input']>;
};

export type AuthRequestWithAccountsFragment = { __typename: 'AuthenticationRequest', id: unknown | null, externalId: string | null, status: AuthenticationStatus, authenticationLink: unknown, createdAt: Date, accessExpiresAt: Date, institution: { __typename: 'BankingInstitution', id: unknown | null, name: string, bic: string, logoUri: unknown } | null, associatedAccounts: Array<{ __typename: 'BankAccount', id: unknown | null, externalId: string | null, iban: string | null, name: string | null, description: string | null, accountType: string | null, balance: number | null, currency: string | null, ownerName: string | null, includeInAnalytics: boolean, associatedInstitution: { __typename: 'BankingInstitution', id: unknown | null, name: string, bic: string, logoUri: unknown } | null, syncStatus: { __typename: 'SyncStatus', state: SyncState, lastSuccessfulSync: Date | null, lastError: string | null, nextRetry: Date | null } }> };

export type BankAccountDetailsFragment = { __typename: 'BankAccount', id: unknown | null, iban: string | null, name: string | null, description: string | null, accountType: string | null, balance: number | null, currency: string | null, ownerName: string | null, includeInAnalytics: boolean, associatedInstitution: { __typename: 'BankingInstitution', name: string, bic: string, logoUri: unknown } | null };

export type BankAccountSummaryFragment = { __typename: 'BankAccount', id: unknown | null, iban: string | null, name: string | null, balance: number | null, currency: string | null, associatedInstitution: { __typename: 'BankingInstitution', name: string, logoUri: unknown } | null, syncStatus: { __typename: 'SyncStatus', state: SyncState, lastSuccessfulSync: Date | null, lastError: string | null, nextRetry: Date | null } };

export type SyncStatusFieldsFragment = { __typename: 'SyncStatus', state: SyncState, lastSuccessfulSync: Date | null, lastError: string | null, nextRetry: Date | null };

export type TransactionFragment = { __typename: 'Transaction', id: unknown | null, valueDate: Date | null, amount: number, payer: string | null, payee: string | null, description: string | null, currency: string, status: TransactionStatus };

export type CreateAuthenticationRequestMutationVariables = Exact<{
  institutionId: Scalars['UUID']['input'];
  redirectUri: Scalars['String']['input'];
}>;


export type CreateAuthenticationRequestMutation = { createAuthenticationRequest: { __typename: 'CreateAuthenticationRequestPayload', authenticationRequest: { __typename: 'AuthenticationRequest', id: unknown | null, externalId: string | null, status: AuthenticationStatus, authenticationLink: unknown, associatedAccounts: Array<{ __typename: 'BankAccount', id: unknown | null }> } | null } };

export type DeleteAuthenticationRequestMutationVariables = Exact<{
  authenticationId: Scalars['UUID']['input'];
}>;


export type DeleteAuthenticationRequestMutation = { deleteAuthenticationRequest: { __typename: 'DeleteAuthenticationRequestPayload', deleteResponse: { __typename: 'DeleteResponse', deletedId: unknown } | null } };

export type SetBankAccountIncludeInAnalyticsMutationVariables = Exact<{
  bankAccountId: Scalars['UUID']['input'];
  includeInAnalytics: Scalars['Boolean']['input'];
}>;


export type SetBankAccountIncludeInAnalyticsMutation = { setBankAccountIncludeInAnalytics: { __typename: 'SetBankAccountIncludeInAnalyticsPayload', bankAccount: { __typename: 'BankAccount', id: unknown | null, includeInAnalytics: boolean } | null } };

export type GetAuthRequestWithAccountsQueryVariables = Exact<{
  authenticationId: Scalars['UUID']['input'];
}>;


export type GetAuthRequestWithAccountsQuery = { authenticationRequest: { __typename: 'AuthenticationRequest', id: unknown | null, externalId: string | null, status: AuthenticationStatus, authenticationLink: unknown, createdAt: Date, accessExpiresAt: Date, institution: { __typename: 'BankingInstitution', id: unknown | null, name: string, bic: string, logoUri: unknown } | null, associatedAccounts: Array<{ __typename: 'BankAccount', id: unknown | null, externalId: string | null, iban: string | null, name: string | null, description: string | null, accountType: string | null, balance: number | null, currency: string | null, ownerName: string | null, includeInAnalytics: boolean, associatedInstitution: { __typename: 'BankingInstitution', id: unknown | null, name: string, bic: string, logoUri: unknown } | null, syncStatus: { __typename: 'SyncStatus', state: SyncState, lastSuccessfulSync: Date | null, lastError: string | null, nextRetry: Date | null } }> } | null };

export type GetAuthRequestsWithAccountsQueryVariables = Exact<{ [key: string]: never; }>;


export type GetAuthRequestsWithAccountsQuery = { authenticationRequests: Array<{ __typename: 'AuthenticationRequest', id: unknown | null, externalId: string | null, status: AuthenticationStatus, authenticationLink: unknown, createdAt: Date, accessExpiresAt: Date, institution: { __typename: 'BankingInstitution', id: unknown | null, name: string, bic: string, logoUri: unknown } | null, associatedAccounts: Array<{ __typename: 'BankAccount', id: unknown | null, externalId: string | null, iban: string | null, name: string | null, description: string | null, accountType: string | null, balance: number | null, currency: string | null, ownerName: string | null, includeInAnalytics: boolean, associatedInstitution: { __typename: 'BankingInstitution', id: unknown | null, name: string, bic: string, logoUri: unknown } | null, syncStatus: { __typename: 'SyncStatus', state: SyncState, lastSuccessfulSync: Date | null, lastError: string | null, nextRetry: Date | null } }> }> };

export type GetAuthenticationRequestQueryVariables = Exact<{
  authenticationId: Scalars['UUID']['input'];
}>;


export type GetAuthenticationRequestQuery = { authenticationRequest: { __typename: 'AuthenticationRequest', id: unknown | null, externalId: string | null, status: AuthenticationStatus, authenticationLink: unknown, associatedAccounts: Array<{ __typename: 'BankAccount', id: unknown | null }> } | null };

export type GetBankAccountQueryVariables = Exact<{
  accountId: Scalars['UUID']['input'];
}>;


export type GetBankAccountQuery = { bankAccount: { __typename: 'BankAccount', id: unknown | null, iban: string | null, name: string | null, description: string | null, accountType: string | null, balance: number | null, currency: string | null, ownerName: string | null, includeInAnalytics: boolean, associatedInstitution: { __typename: 'BankingInstitution', name: string, bic: string, logoUri: unknown } | null } | null };

export type GetBankAccountsQueryVariables = Exact<{ [key: string]: never; }>;


export type GetBankAccountsQuery = { bankAccounts: { __typename: 'BankAccountsConnection', nodes: Array<{ __typename: 'BankAccount', id: unknown | null, iban: string | null, name: string | null, balance: number | null, currency: string | null, associatedInstitution: { __typename: 'BankingInstitution', name: string, logoUri: unknown } | null, syncStatus: { __typename: 'SyncStatus', state: SyncState, lastSuccessfulSync: Date | null, lastError: string | null, nextRetry: Date | null } }> | null } | null };

export type GetBankingInstitutionsQueryVariables = Exact<{
  countryCode?: InputMaybe<Scalars['String']['input']>;
  first?: InputMaybe<Scalars['Int']['input']>;
}>;


export type GetBankingInstitutionsQuery = { bankingInstitutions: { __typename: 'BankingInstitutionsConnection', pageInfo: { __typename: 'PageInfo', hasNextPage: boolean, hasPreviousPage: boolean, startCursor: string | null, endCursor: string | null }, edges: Array<{ __typename: 'BankingInstitutionsEdge', node: { __typename: 'BankingInstitution', id: unknown | null, source: DataSource, name: string, bic: string, logoUri: unknown, countries: Array<string> } }> | null } | null };

export type GetNetWorthHistoryQueryVariables = Exact<{ [key: string]: never; }>;


export type GetNetWorthHistoryQuery = { bankAccounts: { __typename: 'BankAccountsConnection', nodes: Array<{ __typename: 'BankAccount', id: unknown | null, balance: number | null, currency: string | null, syncStatus: { __typename: 'SyncStatus', state: SyncState }, transactions: Array<{ __typename: 'Transaction', valueDate: Date | null, amount: number, status: TransactionStatus }> }> | null } | null };

export type GetTransactionsByAccountIdQueryVariables = Exact<{
  accountId: Scalars['UUID']['input'];
  first?: InputMaybe<Scalars['Int']['input']>;
  after?: InputMaybe<Scalars['String']['input']>;
  last?: InputMaybe<Scalars['Int']['input']>;
  before?: InputMaybe<Scalars['String']['input']>;
}>;


export type GetTransactionsByAccountIdQuery = { transactionsByAccountId: { __typename: 'TransactionsByAccountIdConnection', pageInfo: { __typename: 'PageInfo', hasNextPage: boolean, startCursor: string | null, endCursor: string | null }, edges: Array<{ __typename: 'TransactionsByAccountIdEdge', node: { __typename: 'Transaction', id: unknown | null, valueDate: Date | null, amount: number, payer: string | null, payee: string | null, description: string | null, currency: string, status: TransactionStatus } }> | null } | null };

export type DataSynchronizedSubscriptionVariables = Exact<{ [key: string]: never; }>;


export type DataSynchronizedSubscription = { dataSynchronized: { __typename: 'DataSynchronization', completedAt: Date } };
