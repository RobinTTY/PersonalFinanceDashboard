import { StoryWrapper } from '../storybook/StoryWrapper';
import attributes from './attributes.json';
import { Navbar } from './Navbar';

export default { title: 'Navbar' };

export function Usage() {
  return <StoryWrapper attributes={attributes} component={Navbar} />;
}
