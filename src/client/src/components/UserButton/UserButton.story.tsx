import { ComponentPreview } from '../storybook/ComponentPreview';
import { UserButton } from './UserButton';

export default { title: 'UserButton' };

export function Usage() {
  return (
    <ComponentPreview canvas={{ center: true, maxWidth: 320 }} withSpacing>
      <UserButton />
    </ComponentPreview>
  );
}
