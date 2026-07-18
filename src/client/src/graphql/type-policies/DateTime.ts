import { FieldPolicy } from '@apollo/client';
import { isNullOrUndefined } from '@utility';

export const dateTimeTypePolicy: FieldPolicy<Date, string | Date> = {
  merge: (_, incoming) => {
    if (isNullOrUndefined(incoming)) {
      return incoming;
    }

    if (incoming instanceof Date) {
      return incoming;
    }

    return new Date(incoming);
  },
};
