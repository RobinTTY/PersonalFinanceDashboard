import { FieldPolicy } from '@apollo/client';
import { isNullOrUndefined } from '@/utility/isNullOrUndefined';

export const dateTimeTypePolicy: FieldPolicy<Date, string | Date> = {
  merge: (_, incoming) => {
    if (isNullOrUndefined(incoming)) {
      // It's important for these methods to return null if passed null
      return incoming;
    }

    if (incoming instanceof Date) {
      // TODO: may remove this
      // In tests our mocks already have Date
      return incoming;
    }

    return new Date(incoming);
  },
};
