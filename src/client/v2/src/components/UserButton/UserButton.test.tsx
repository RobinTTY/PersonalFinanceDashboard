import { axe, render } from '@test-utils';
import { ComponentPreview } from '../storybook/ComponentPreview';
import { UserButton } from './UserButton';

const attributes = { center: true, maxWidth: 320 };

describe('UserButton', () => {
  axe([
    <ComponentPreview key="1" canvas={attributes} withSpacing>
      <UserButton />
    </ComponentPreview>,
  ]);

  it('renders correctly', () => {
    render(
      <ComponentPreview canvas={attributes} withSpacing>
        <UserButton />
      </ComponentPreview>,
    );
  });
});
