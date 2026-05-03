import { AuthenticationRequest } from '@/graphql/types/graphql';

/**
 * Props for the AuthenticationStep component.
 */
export interface AuthenticationStepProps {
  /**
   * Id of the bank to authenticate with.
   */
  bankId: string;
  /**
   * Callback to call when the user has finished authenticating.
   * @param authentication Data associated with the authentication.
   * @returns void
   */
  onFinishSetup: (authentication: AuthenticationRequest) => void;
}
