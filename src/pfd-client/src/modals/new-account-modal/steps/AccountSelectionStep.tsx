import { Text } from '@mantine/core';
import { Authentication } from './AuthenticationStep';

export const AccountSelectionStep = ({ authentication }: AccountSelectionStepProps) => {
  console.log(authentication);

  return <Text>Account Selection</Text>;
};

export interface AccountSelectionStepProps {
  authentication: Authentication;
}
