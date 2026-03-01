import { axe, render } from '@test-utils';
import attributes from './attributes.json';
import { Navbar } from './Navbar';

describe('Navbar', () => {
  axe([<Navbar key="1" {...(attributes as any)} />]);

  it('renders correctly', () => {
    render(<Navbar {...(attributes as any)} />);
  });
});
