import { dateTimeTypePolicy } from "../type-policies/DateTime.js";

export const scalarTypePolicies = { Transaction: { fields: { valueDate: dateTimeTypePolicy } } };
