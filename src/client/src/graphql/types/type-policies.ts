import { dateTimeTypePolicy } from '../type-policies/DateTime.js';

export const scalarTypePolicies = {
  AuthenticationRequest: {
    fields: { accessExpiresAt: dateTimeTypePolicy, createdAt: dateTimeTypePolicy },
  },
  SyncStatus: { fields: { lastSuccessfulSync: dateTimeTypePolicy, nextRetry: dateTimeTypePolicy } },
  Transaction: { fields: { valueDate: dateTimeTypePolicy } },
};
