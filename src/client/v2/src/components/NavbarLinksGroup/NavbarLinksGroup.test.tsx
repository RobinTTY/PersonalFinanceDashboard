import { IconCalendarStats } from '@tabler/icons-react';
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
  axe([<NavbarLinksGroup key="1" {...mockdata} />]);

  it('renders correctly', () => {
    render(<NavbarLinksGroup {...mockdata} />);
  });
});
