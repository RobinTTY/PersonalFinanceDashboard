import { useEffect, useState } from 'react';
import { useMutation } from '@apollo/client/react';
import { Button, CloseButton, Modal, Stack, Text, Title } from '@mantine/core';
import { useFlushAnalyticsRefetchOnClose } from '@/components/IncludeInAnalyticsSwitch/DeferredAnalyticsRefetch';
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

/** A bank whose expired connection is renewed. */
export interface ReconnectInstitution {
  /** The internal id of the institution. */
  id: string;
  name: string;
}

interface AddAccountModalProps {
  opened: boolean;
  onClose: () => void;
  pendingAuth?: PendingAuthState;
  /** Skips choosing the account type and bank, and renews the connection to this bank instead. */
  reconnect?: ReconnectInstitution;
}

export function AddAccountModal({ opened, onClose, pendingAuth, reconnect }: AddAccountModalProps) {
  useFlushAnalyticsRefetchOnClose(opened);
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

  const [createAuthenticationRequest, { loading: createAuthLoading }] = useMutation(
    CreateAuthenticationRequest
  );

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

  useEffect(() => {
    if (opened && reconnect) {
      setSelectedBank(reconnect.id);
      setStep(2);
    }
  }, [opened, reconnect]);

  const handleSynchronizeWithBank = async () => {
    if (!selectedBank) {
      return;
    }

    let result;
    try {
      result = await createAuthenticationRequest({
        variables: { institutionId: selectedBank, redirectUri: window.location.href },
      });
    } catch {
      // The error is shown as a notification by the error link
      return;
    }

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
    2: reconnect
      ? {
          title: `Reconnect ${reconnect.name}`,
          subtitle: 'Renew the access to your accounts at this bank.',
        }
      : { title: 'Select Bank', subtitle: 'Choose the bank you want to synchronize.' },
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
        body:
          step === 2 && !reconnect
            ? `${classes.modalBody} ${classes.modalBodyLarge}`
            : classes.modalBody,
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

            {step === 2 &&
              (reconnect ? (
                <Text size="sm">
                  You'll be forwarded to {reconnect.name} through GoCardless to confirm the access
                  again. Your accounts keep their transactions and settings.
                </Text>
              ) : (
                <SelectBankStep selectedBank={selectedBank} onBankSelect={setSelectedBank} />
              ))}

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
                <Button variant="default" onClick={reconnect ? handleClose : handleBack}>
                  {reconnect ? 'Cancel' : 'Back'}
                </Button>
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
