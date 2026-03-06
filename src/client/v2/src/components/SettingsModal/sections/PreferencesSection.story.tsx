import { ComponentPreview } from '../../storybook/ComponentPreview';
import attributes from './attributes.json';
import { PreferencesSection } from './PreferencesSection';

export default { title: 'Settings/PreferencesSection' };

export function Usage() {
  return (
    <ComponentPreview canvas={attributes.canvas} withSpacing>
      <PreferencesSection />
    </ComponentPreview>
  );
}
