export const isNullOrUndefined = <T>(obj: T | null | undefined): obj is null | undefined =>
  typeof obj === 'undefined' || obj === null;
