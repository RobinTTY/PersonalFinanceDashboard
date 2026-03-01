import { StoryWrapper } from '../storybook/StoryWrapper';
import attributes from './attributes.json';
import { UserButton } from './UserButton';

export default { title: 'UserButton' };

export function Usage() {
  return <StoryWrapper attributes={attributes} component={UserButton} />;
}
