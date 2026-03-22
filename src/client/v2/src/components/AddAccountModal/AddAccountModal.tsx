import { useState } from 'react';
import { Button, CloseButton, Modal, Stack, Text, Title } from '@mantine/core';
import { AccountType, SelectAccountTypeStep } from './steps/SelectAccountTypeStep';
import { SelectBankStep } from './steps/SelectBankStep';
import classes from './AddAccountModal.module.css';

interface AddAccountModalProps {
  opened: boolean;
  onClose: () => void;
}

export function AddAccountModal({ opened, onClose }: AddAccountModalProps) {
  const [step, setStep] = useState<1 | 2>(1);
  const [selectedType, setSelectedType] = useState<AccountType | undefined>(undefined);
  const [selectedBank, setSelectedBank] = useState<string | undefined>(undefined);

  const handleClose = () => {
    setStep(1);
    setSelectedType(undefined);
    setSelectedBank(undefined);
    onClose();
  };

  const handleBack = () => {
    setStep(1);
    setSelectedBank(undefined);
  };

  const stepConfig = {
    1: { title: 'Choose Account Type', subtitle: 'Select the type of account you want to add.' },
    2: { title: 'Select Bank', subtitle: 'Choose the bank you want to synchronize.' },
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
              <SelectAccountTypeStep selectedType={selectedType} onChange={setSelectedType} />
            )}

            {step === 2 && (
              <SelectBankStep selectedBank={selectedBank} onBankSelect={setSelectedBank} />
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
            ) : (
              <>
                <Button variant="default" onClick={handleBack}>
                  Back
                </Button>
                <Button disabled={!selectedBank}>Synchronize</Button>
              </>
            )}
          </div>
        </Modal.Body>
      </Modal.Content>
    </Modal.Root>
  );
}
