import { IconCalendarStats } from '@tabler/icons-react';
import { MemoryRouter } from 'react-router-dom';
import { axe, render } from '@test-utils';
import { NavbarLinksGroup } from './NavbarLinksGroup';

const mockdata = {
  label: 'Releases',
  icon: IconCalendarStats,
  links: [
    { label: 'Upcoming releases', link: '/' },
    { label: 'Previous releases', link: '/' },
    { label: 'Releases schedule', link: '/' },
  ],
};

describe('NavbarLinksGroup', () => {
  axe([<MemoryRouter key="1"><NavbarLinksGroup {...mockdata} /></MemoryRouter>]);

  it('renders correctly', () => {
    render(<MemoryRouter><NavbarLinksGroup {...mockdata} /></MemoryRouter>);
  });
});
