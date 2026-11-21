import { dateTimeTypePolicy } from '../type-policies/DateTime.js';

export const scalarTypePolicies = {
  AuthenticationRequest: { fields: { createdAt: dateTimeTypePolicy } },
  SyncStatus: { fields: { lastSuccessfulSync: dateTimeTypePolicy, nextRetry: dateTimeTypePolicy } },
  Transaction: { fields: { valueDate: dateTimeTypePolicy } },
};
