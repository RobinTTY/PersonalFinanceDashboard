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
    <Group
      sx={{
        display: "grid",
        gridTemplateColumns: "1fr 1fr",
      }}
      p="xs"
    >
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

const CountrySelectionStep = ({
  onCountrySelect,
}: CountrySelectionStepProps) => {
  return (
    <ModalOptionSearchList
      options={countries}
      searchPlaceholder="Filter countries..."
      onOptionSelect={(optionKey) => {
        onCountrySelect(optionKey);
      }}
    />
  );
};

interface CountrySelectionStepProps {
  onCountrySelect: (country: string) => void;
}

const banks = [
  {
    key: "DORTMUNDER_VOLKSBANK_GENODEM1DOR",
    description: "Dortmunder Volksbank",
    icon: (
      <img
        src="https://cdn.nordigen.com/ais/VOLKSBANK_NIEDERGRAFSCHAFT_GENODEF1HOO.png"
        alt="Dortmunder Volksbank"
      />
    ),
  },
  {
    key: "KSK_BOBLINGEN_BBKRDE6BXXX",
    description: "Kreissparkasse Böblingen",
    icon: (
      <img
        src="https://storage.googleapis.com/gc-prd-institution_icons-production/DE/PNG/sparkasse.png"
        alt="Kreissparkasse Böblingen"
      />
    ),
  },
  {
    key: "COMMERZBANK_COBADEFF",
    description: "Commerzbank",
    icon: (
      <img
        src="https://storage.googleapis.com/gc-prd-institution_icons-production/DE/PNG/commerzbank.png"
        alt="Commerzbank"
      />
    ),
  },
  {
    key: "ING_INGDDEFF",
    description: "ING",
    icon: (
      <img
        src="https://storage.googleapis.com/gc-prd-institution_icons-production/DE/PNG/ing.png"
        alt="ING"
      />
    ),
  },
  {
    key: "DKB_BYLADEM1",
    description: "Deutsche Kreditbank AG (DKB)",
    icon: (
      <img
        src="https://storage.googleapis.com/gc-prd-institution_icons-production/DE/PNG/deutschekreditbank.png"
        alt="Deutsche Kreditbank AG (DKB)"
      />
    ),
  },
  {
    key: "DEUTSCHE_BANK_DEUTDEFF",
    description: "Deutsche Bank",
    icon: (
      <img
        src="https://storage.googleapis.com/gc-prd-institution_icons-production/DE/PNG/deutschebank.png"
        alt="Deutsche Bank"
      />
    ),
  },
];

const BankSelectionStep = () => {
  return (
    <ModalOptionSearchList
      options={banks}
      searchPlaceholder="Filter banks..."
      optionDescriptionWidth={200}
      truncateOptionDescription
      onOptionSelect={(optionKey) => {}}
    />
  );
};

export const NewAccountModal = ({
  opened,
  closeModal,
}: NewAccountModalProps) => {
  const [addAccountStep, addAccountStepHandler] = useCounter(1, {
    min: 1,
    max: 4,
  });
  const [accountType, setAccountType] = useState<AccountType>();
  const [country, setCountry] = useState<string>();

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
              <CountrySelectionStep
                onCountrySelect={(optionKey) => {
                  setCountry(optionKey);
                  addAccountStepHandler.increment();
                }}
              />
            );
          case "investment":
            return <Text>Step 2</Text>;
          default:
            throw new Error("Invalid account type");
        }
      case 3:
        return <BankSelectionStep />;
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
          {addAccountStep === 3 && "Select your bank"}
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
