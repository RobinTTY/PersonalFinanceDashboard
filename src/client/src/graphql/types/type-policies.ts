import { dateTimeTypePolicy } from "../type-policies/DateTime";

export const scalarTypePolicies = { Transaction: { fields: { valueDate: dateTimeTypePolicy } } };
