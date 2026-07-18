import { gql, TypedDocumentNode } from '@apollo/client';
import { DeleteAuthenticationRequestMutation } from '../types/graphql';

export const DeleteAuthenticationRequest: TypedDocumentNode<DeleteAuthenticationRequestMutation> =
  gql(`
  mutation DeleteAuthenticationRequest($authenticationId: UUID!) {
    deleteAuthenticationRequest(input: { authenticationId: $authenticationId }) {
      deleteResponse {
        deletedId
      }
    }
  }
`);
