import { gql, TypedDocumentNode } from '@apollo/client';
import { DataSynchronizedSubscription } from '../types/graphql';

export const DataSynchronized: TypedDocumentNode<DataSynchronizedSubscription> = gql`
  subscription DataSynchronized {
    dataSynchronized {
      completedAt
    }
  }
`;
