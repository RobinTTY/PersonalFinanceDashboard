import { useEffect, useState } from 'react';
import { useMutation } from '@apollo/client/react';
import { Button, CloseButton, Modal, Stack, Text, Title } from '@mantine/core';
import { CreateAuthenticationRequest } from '@/graphql/mutations/CreateAuthenticationRequest';
import { AuthenticateBankStep } from './steps/AuthenticateBankStep/AuthenticateBankStep';
import {
  AccountType,
  SelectAccountTypeStep,
} from './steps/SelectAccountTypeStep/SelectAccountTypeStep';
import { SelectBankStep } from './steps/SelectBankStep/SelectBankStep';
import classes from './AddAccountModal.module.css';

const STORAGE_KEY = 'pendingGoCardlessAuth';

export interface PendingAuthState {
  authenticationId: string;
  authenticationLink: string;
}

interface AddAccountModalProps {
  opened: boolean;
  onClose: () => void;
  pendingAuth?: PendingAuthState;
}

export function AddAccountModal({ opened, onClose, pendingAuth }: AddAccountModalProps) {
  const [step, setStep] = useState<1 | 2 | 3>(1);
  const [selectedType, setSelectedAccountType] = useState<AccountType | undefined>(undefined);
  const [selectedBank, setSelectedBank] = useState<string | undefined>(undefined);
  const [authState, setAuthState] = useState<{
    id: string | undefined;
    link: string | undefined;
    isAuthenticated: boolean;
    autoCheck: boolean;
  }>({
    id: undefined,
    link: undefined,
    isAuthenticated: false,
    autoCheck: false,
  });

  const [createAuthenticationRequest, { loading: createAuthLoading, error: createAuthError }] =
    useMutation(CreateAuthenticationRequest);

  /**
   * If the user has completed the GoCardless authentication flow but hasn't returned to the app yet, we can detect that via localStorage and skip them directly to the final step of the flow.
   * By storing the pending authentication state in localStorage, we can check for it when the modal opens and update the UI accordingly.
   */
  useEffect(() => {
    if (pendingAuth) {
      setAuthState({
        id: pendingAuth.authenticationId,
        link: pendingAuth.authenticationLink,
        isAuthenticated: false,
        autoCheck: true,
      });
      setStep(3);
    }
  }, [pendingAuth]);

  const handleSynchronizeWithBank = async () => {
    if (!selectedBank) {
      return;
    }

    const result = await createAuthenticationRequest({
      variables: { institutionId: selectedBank, redirectUri: window.location.href },
    });

    const id = result.data?.createAuthenticationRequest?.authenticationRequest?.id;
    const link =
      result.data?.createAuthenticationRequest?.authenticationRequest?.authenticationLink;

    if (id) {
      const authId = String(id);
      const authLink = link ? String(link) : undefined;
      setAuthState({
        id: authId,
        link: authLink,
        isAuthenticated: false,
        autoCheck: false,
      });

      if (authLink) {
        localStorage.setItem(
          STORAGE_KEY,
          JSON.stringify({ authenticationId: authId, authenticationLink: authLink })
        );
        window.open(authLink, '_blank', 'noopener,noreferrer');
      }

      setStep(3);
    }
  };

  const handleClose = () => {
    localStorage.removeItem(STORAGE_KEY);
    setStep(1);
    setSelectedAccountType(undefined);
    setSelectedBank(undefined);
    setAuthState({
      id: undefined,
      link: undefined,
      isAuthenticated: false,
      autoCheck: false,
    });
    onClose();
  };

  const handleBack = () => {
    setStep(1);
    setSelectedBank(undefined);
  };

  const stepConfig = {
    1: { title: 'Choose Account Type', subtitle: 'Select the type of account you want to add.' },
    2: { title: 'Select Bank', subtitle: 'Choose the bank you want to synchronize.' },
    3: {
      title: 'Authenticate with GoCardless',
      subtitle: 'Authenticate with your bank through GoCardless to complete the setup.',
    },
  };

  const { title, subtitle } = stepConfig[step];

  return (
    <Modal.Root
      opened={opened}
      onClose={handleClose}
      size={700}
      padding={0}
      centered
      classNames={{
        content: classes.modalContent,
        body: step === 2 ? `${classes.modalBody} ${classes.modalBodyLarge}` : classes.modalBody,
      }}
    >
      <Modal.Overlay />
      <Modal.Content aria-label="Add Account">
        <Modal.Body>
          <div className={classes.header}>
            <Stack gap={2}>
              <Title order={3}>{title}</Title>
              <Text size="sm" c="dimmed">
                {subtitle}
              </Text>
            </Stack>
            <CloseButton onClick={handleClose} size="md" aria-label="Close" />
          </div>

          <div className={classes.content}>
            {step === 1 && (
              <SelectAccountTypeStep
                selectedType={selectedType}
                onChange={setSelectedAccountType}
              />
            )}

            {step === 2 && (
              <SelectBankStep selectedBank={selectedBank} onBankSelect={setSelectedBank} />
            )}

            {step === 3 && (
              <AuthenticateBankStep
                authenticationId={authState.id}
                authenticationLink={authState.link}
                autoCheck={authState.autoCheck}
                onAuthenticated={() => setAuthState((prev) => ({ ...prev, isAuthenticated: true }))}
              />
            )}
          </div>

          <div className={classes.footer}>
            {step === 1 ? (
              <>
                <Button variant="default" onClick={handleClose}>
                  Cancel
                </Button>
                <Button disabled={!selectedType} onClick={() => setStep(2)}>
                  Next
                </Button>
              </>
            ) : step === 2 ? (
              <>
                <Button variant="default" onClick={handleBack}>
                  Back
                </Button>
                {/* TODO: this auth error should be displayed in a notification component, not inline */}
                {createAuthError && (
                  <Text c="red" size="sm" mr="auto">
                    {createAuthError.message}
                  </Text>
                )}
                <Button
                  disabled={!selectedBank}
                  loading={createAuthLoading}
                  onClick={handleSynchronizeWithBank}
                >
                  Synchronize
                </Button>
              </>
            ) : authState.isAuthenticated ? (
              <Button mx="auto" onClick={handleClose}>
                Done
              </Button>
            ) : null}
          </div>
        </Modal.Body>
      </Modal.Content>
    </Modal.Root>
  );
}
