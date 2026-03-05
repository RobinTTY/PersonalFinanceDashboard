import {
  IconAdjustments,
  IconCalendarStats,
  IconGauge,
  IconNotes,
  IconPresentationAnalytics,
} from '@tabler/icons-react';
import { useDisclosure } from '@mantine/hooks';
import { Outlet } from 'react-router-dom';
import { Navbar } from '@/components/Navbar/Navbar';
import { LinksGroupProps } from '@/components/NavbarLinksGroup/NavbarLinksGroup';
import { SettingsModal } from '@/components/SettingsModal/SettingsModal';

export function AppLayout() {
  const [settingsOpened, { open: openSettings, close: closeSettings }] = useDisclosure(false);

  const navLinks: LinksGroupProps[] = [
    { label: 'Dashboard', icon: IconGauge, link: '/dashboard' },
    { label: 'Accounts', icon: IconNotes, link: '/accounts' },
    { label: 'Transactions', icon: IconCalendarStats, link: '/transactions' },
    { label: 'Analytics', icon: IconPresentationAnalytics, link: '/analytics' },
    { label: 'Settings', icon: IconAdjustments, onClick: openSettings },
  ];

  return (
    <div style={{ display: 'flex', height: '100vh' }}>
      <Navbar links={navLinks} />
      <main style={{ flex: 1, overflow: 'auto', padding: 'var(--mantine-spacing-md)' }}>
        <Outlet />
      </main>
      <SettingsModal opened={settingsOpened} onClose={closeSettings} />
    </div>
  );
}
