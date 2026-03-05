import { useState } from 'react';
import {
  IconAdjustments,
  IconBell,
  IconFileImport,
  IconInfoCircle,
  IconLink,
  IconTable,
  IconUser,
} from '@tabler/icons-react';
import { CloseButton, Modal, NavLink, ScrollArea, Text, Title } from '@mantine/core';
import classes from './SettingsModal.module.css';

interface SettingsItem {
  label: string;
  icon: React.FC<{ size?: number }>;
}

interface SettingsSection {
  label: string;
  items: SettingsItem[];
}

const sections: SettingsSection[] = [
  {
    label: 'Account',
    items: [
      { label: 'My Profile', icon: IconUser },
      { label: 'Preferences', icon: IconAdjustments },
      { label: 'Notifications', icon: IconBell },
      { label: 'Connections', icon: IconLink },
    ],
  },
  {
    label: 'Application',
    items: [
      { label: 'General', icon: IconTable },
      { label: 'Import', icon: IconFileImport },
    ],
  },
  {
    label: 'About',
    items: [{ label: 'About', icon: IconInfoCircle }],
  },
];

interface SettingsModalProps {
  opened: boolean;
  onClose: () => void;
}

export function SettingsModal({ opened, onClose }: SettingsModalProps) {
  const [activeLabel, setActiveLabel] = useState('Preferences');

  return (
    <Modal
      opened={opened}
      onClose={onClose}
      size={1000}
      padding={0}
      withCloseButton={false}
      centered
      classNames={{
        content: classes.modalContent,
        body: classes.modalBody,
      }}
    >
      <ScrollArea className={classes.sidebar} h="100%">
        {sections.map((section, index) => (
          <div key={section.label}>
            <Text
              className={classes.sectionLabel}
              style={index === 0 ? { paddingTop: 'var(--mantine-spacing-xs)' } : undefined}
            >
              {section.label}
            </Text>
            {section.items.map((item) => (
              <NavLink
                key={item.label}
                label={item.label}
                leftSection={<item.icon size={16} />}
                active={activeLabel === item.label}
                onClick={() => setActiveLabel(item.label)}
              />
            ))}
          </div>
        ))}
      </ScrollArea>

      <div className={classes.contentArea}>
        <div className={classes.contentHeader}>
          <Title order={3}>{activeLabel}</Title>
          <CloseButton onClick={onClose} size="md" aria-label="Close" />
        </div>
        <ScrollArea className={classes.contentBody} h="100%">
          {/* Settings content */}
        </ScrollArea>
      </div>
    </Modal>
  );
}
