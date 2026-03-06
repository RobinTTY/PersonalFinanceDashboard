import { IconCalendarStats } from '@tabler/icons-react';
import { MemoryRouter } from 'react-router-dom';
import { ComponentPreview } from '../storybook/ComponentPreview';
import { NavbarLinksGroup } from './NavbarLinksGroup';

export default {
  title: 'NavbarLinksGroup',
  decorators: [(Story: any) => <MemoryRouter><Story /></MemoryRouter>],
};

const mockdata = {
  label: 'Releases',
  icon: IconCalendarStats,
  links: [
    { label: 'Upcoming releases', link: '/' },
    { label: 'Previous releases', link: '/' },
    { label: 'Releases schedule', link: '/' },
  ],
};

export function Usage() {
  return (
    <ComponentPreview canvas={{ center: true, maxWidth: 320 }} withSpacing>
      <NavbarLinksGroup {...mockdata} />
    </ComponentPreview>
  );
}
