import { dateTimeTypePolicy } from "../type-policies/DateTime.js";

export const scalarTypePolicies = {
  AuthenticationRequest: { fields: { createdAt: dateTimeTypePolicy } },
  Transaction: { fields: { valueDate: dateTimeTypePolicy } },
};
