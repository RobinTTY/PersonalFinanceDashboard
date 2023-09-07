import { Group, Modal, Text } from "@mantine/core";
import { useCounter } from "@mantine/hooks";
import { IconCoins, IconGraph } from "@tabler/icons-react";
import { DE, US, CA, GB, AU, NZ } from "country-flag-icons/react/3x2";

import { ModalButton } from "../components/modal-button/ModalButton";
import { ModalOptionSearchList } from "../components/modal-option-search-list/ModalOptionSearchList";
import { useState } from "react";

type AccountType = "savings" | "investment";

const AccountTypeSelectionStep = ({
  selectionAction,
}: AccountTypeSelectionStepProps) => {
  return (
    <Group className="account-type-selection" p="xs">
      {/* TODO: Create reusable component */}
      <ModalButton
        icon={<IconCoins size="4rem" />}
        description="Savings Account"
        action={() => selectionAction("savings")}
      />
      <ModalButton
        icon={<IconGraph size="4rem" />}
        description="Investment Account"
        action={() => selectionAction("investment")}
      />
    </Group>
  );
};

interface AccountTypeSelectionStepProps {
  selectionAction: (accountType: AccountType) => void;
}

const countries = [
  { key: "US", description: "United States", icon: <US /> },
  { key: "CA", description: "Canada", icon: <CA /> },
  { key: "GB", description: "United Kingdom", icon: <GB /> },
  { key: "AU", description: "Australia", icon: <AU /> },
  { key: "NZ", description: "New Zealand", icon: <NZ /> },
  { key: "DE", description: "Germany", icon: <DE /> },
];

export const NewAccountModal = ({
  opened,
  closeModal,
}: NewAccountModalProps) => {
  const [accountType, setAccountType] = useState<AccountType>();
  const [addAccountStep, addAccountStepHandler] = useCounter(1, {
    min: 1,
    max: 4,
  });

  const onNewAccount = (accountType: AccountType) => {
    setAccountType(accountType);
    addAccountStepHandler.increment();
  };

  const getActiveNewAccountStep = () => {
    switch (addAccountStep) {
      case 1:
        return <AccountTypeSelectionStep selectionAction={onNewAccount} />;
      case 2:
        switch (accountType) {
          case "savings":
            return (
              <ModalOptionSearchList
                options={countries}
                searchPlaceholder="Filter countries..."
                onOptionSelect={(optionKey) => {
                  console.log(optionKey);
                }}
              />
            );
          case "investment":
            return <Text>Step 2</Text>;
          default:
            throw new Error("Invalid account type");
        }
      default:
        return <Text>Step {addAccountStep}</Text>;
    }
  };

  return (
    <Modal
      opened={opened}
      onClose={() => {
        closeModal();
        addAccountStepHandler.reset();
      }}
      title={
        <Text fz="lg" fw={700} pl=".4rem">
          {addAccountStep === 1 &&
            "What type of account would you like to add?"}
          {addAccountStep === 2 && "Select your country"}
        </Text>
      }
      centered
      size="auto"
    >
      {getActiveNewAccountStep()}
    </Modal>
  );
};

interface NewAccountModalProps {
  opened: boolean;
  closeModal: () => void;
}
