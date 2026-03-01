import { useState } from 'react';
import { useLazyQuery, useMutation } from "@apollo/client/react";
import { useDisclosure } from '@mantine/hooks';
import { Button, Center, Stack, Text } from '@mantine/core';
import { CreateAuthenticationRequestMutation } from '@graphql-mutations/CreateAuthenticationRequest';
import { GetAuthenticationRequestQuery } from '@graphql-queries/GetAuthenticationRequest';
import { AuthenticationStepProps } from './AuthenticationStepProps';

// TODO: The redirect could go to a page that indicates that authentication was sucessful and tab can be closed
// TODO: logo needs to depend on theme (light/dark)
export const AuthenticationStep = ({ bankId, onFinishSetup }: AuthenticationStepProps) => {
  // const sanboxInstitution = 'SANDBOXFINANCE_SFIN0000';
  const [buttonDescription, setButtonDescription] = useState('Start Authentication');
  const [loading, loadingHandler] = useDisclosure(false);
  const [createAuthenticationRequest, { data: authRequestData }] = useMutation(
    CreateAuthenticationRequestMutation,
    {
      variables: {
        institutionId: bankId,
        redirectUri: 'https://www.robintty.com/',
      },
      onCompleted: (data) => {
        const link = data.createAuthenticationRequest.authenticationLink;
        window.open(link, '_blank')?.focus();
      },
    }
  );

  const [
    getAuthenticationRequest,
    // { loading: accountLoading, error: accountError, data: accountData },
  ] = useLazyQuery(GetAuthenticationRequestQuery);

  return (
    <Stack p="md" miw={275}>
      <Center>
        <img
          width="192px"
          src="/src/assets/img/go-cardless/logo-negative.svg"
          alt="go-cardless-logo"
        />
      </Center>
      {/* TODO: Provider should be link? */}
      <Center>
        <Text fw={500}>Data Provider: GoCardless</Text>
      </Center>
      <Button
        size="lg"
        fullWidth
        loading={loading}
        onClick={async () => {
          // TODO: This could maybe be done more elegantly?
          // Second button interaction: Once user is redirected back from GoCardless
          if (authRequestData) {
            // TODO: Handle status other than success
            loadingHandler.open();
            const auth = await getAuthenticationRequest({
              variables: { authenticationId: authRequestData.createAuthenticationRequest.id },
            });
            loadingHandler.close();

            onFinishSetup(auth.data?.authenticationRequest!);
            // First button interaction: Create authentication request
          } else {
            createAuthenticationRequest();
            loadingHandler.open();
            setTimeout(() => {
              loadingHandler.close();
              setButtonDescription('Finish Setup...');
            }, 2000);
          }
        }}
      >
        {buttonDescription}
      </Button>
    </Stack>
  );
};
