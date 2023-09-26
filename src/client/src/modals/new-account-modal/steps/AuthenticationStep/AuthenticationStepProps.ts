import { AuthenticationRequest } from '@/graphql/types/graphql';

export interface AuthenticationStepProps {
  onFinishSetup: (authentication: AuthenticationRequest) => void;
}
