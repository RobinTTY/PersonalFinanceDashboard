import {
  IconAdjustments,
  IconCalendarStats,
  IconGauge,
  IconNotes,
  IconPresentationAnalytics,
} from '@tabler/icons-react';
import { Outlet } from 'react-router-dom';
import { Navbar } from '@/components/Navbar/Navbar';
import { LinksGroupProps } from '@/components/NavbarLinksGroup/NavbarLinksGroup';

const navLinks: LinksGroupProps[] = [
  { label: 'Dashboard', icon: IconGauge, link: '/dashboard' },
  { label: 'Accounts', icon: IconNotes, link: '/accounts' },
  { label: 'Transactions', icon: IconCalendarStats, link: '/transactions' },
  { label: 'Analytics', icon: IconPresentationAnalytics, link: '/analytics' },
  { label: 'Settings', icon: IconAdjustments, link: '/settings' },
];

export function AppLayout() {
  return (
    <div style={{ display: 'flex', height: '100vh' }}>
      <Navbar links={navLinks} />
      <main style={{ flex: 1, overflow: 'auto', padding: 'var(--mantine-spacing-md)' }}>
        <Outlet />
      </main>
    </div>
  );
}
