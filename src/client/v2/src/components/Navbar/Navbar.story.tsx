import {
  IconAdjustments,
  IconCalendarStats,
  IconGauge,
  IconNotes,
  IconPresentationAnalytics,
} from '@tabler/icons-react';
import { MemoryRouter } from 'react-router-dom';
import { ComponentPreview } from '../storybook/ComponentPreview';
import { Navbar } from './Navbar';

export default {
  title: 'Navbar',
  decorators: [(Story: any) => <MemoryRouter><Story /></MemoryRouter>],
};

const mockLinks = [
  { label: 'Dashboard', icon: IconGauge },
  {
    label: 'Accounts',
    icon: IconNotes,
    initiallyOpened: true,
    links: [
      { label: 'Overview', link: '/' },
      { label: 'Forecasts', link: '/' },
      { label: 'Outlook', link: '/' },
      { label: 'Real time', link: '/' },
    ],
  },
  {
    label: 'Transactions',
    icon: IconCalendarStats,
    links: [
      { label: 'Upcoming releases', link: '/' },
      { label: 'Previous releases', link: '/' },
      { label: 'Releases schedule', link: '/' },
    ],
  },
  { label: 'Analytics', icon: IconPresentationAnalytics },
  { label: 'Settings', icon: IconAdjustments },
];

export function Usage() {
  return (
    <ComponentPreview canvas={{ center: false }} withSpacing>
      <Navbar links={mockLinks} />
    </ComponentPreview>
  );
}
