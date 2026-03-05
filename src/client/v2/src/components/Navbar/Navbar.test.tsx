import {
  IconAdjustments,
  IconCalendarStats,
  IconGauge,
  IconNotes,
  IconPresentationAnalytics,
} from '@tabler/icons-react';
import { MemoryRouter } from 'react-router-dom';
import { axe, render } from '@test-utils';
import { Navbar } from './Navbar';

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

describe('Navbar', () => {
  axe([<MemoryRouter key="1"><Navbar links={mockLinks} /></MemoryRouter>]);

  it('renders correctly', () => {
    render(<MemoryRouter><Navbar links={mockLinks} /></MemoryRouter>);
  });
});
