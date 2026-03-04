import {
  IconAdjustments,
  IconCalendarStats,
  IconGauge,
  IconNotes,
  IconPresentationAnalytics,
} from '@tabler/icons-react';
import { ComponentPreview } from '../storybook/ComponentPreview';
import attributes from './attributes.json';
import { Navbar } from './Navbar';

export default { title: 'Navbar' };

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
    <ComponentPreview canvas={attributes.canvas} withSpacing>
      <Navbar links={mockLinks} />
    </ComponentPreview>
  );
}
