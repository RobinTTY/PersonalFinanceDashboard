import { IconGauge, IconNotes, IconPresentationAnalytics } from '@tabler/icons-react';
import { Outlet } from 'react-router-dom';
import { useDisclosure } from '@mantine/hooks';
import { Navbar } from '@/components/Navbar/Navbar';
import { LinksGroupProps } from '@/components/NavbarLinksGroup/NavbarLinksGroup';
import { SettingsModal } from '@/components/SettingsModal/SettingsModal';

export function AppLayout() {
  const [settingsOpened, { open: openSettings, close: closeSettings }] = useDisclosure(false);

  const navLinks: LinksGroupProps[] = [
    { label: 'Dashboard', icon: IconGauge, link: '/dashboard' },
    { label: 'Accounts', icon: IconNotes, link: '/accounts' },
    { label: 'Analytics', icon: IconPresentationAnalytics, link: '/analytics' },
  ];

  return (
    <div style={{ display: 'flex', height: '100vh' }}>
      <Navbar links={navLinks} onUserProfileClick={openSettings} />
      <main style={{ flex: 1, overflow: 'auto', padding: 'var(--mantine-spacing-md)' }}>
        <Outlet />
      </main>
      <SettingsModal opened={settingsOpened} onClose={closeSettings} />
    </div>
  );
}
