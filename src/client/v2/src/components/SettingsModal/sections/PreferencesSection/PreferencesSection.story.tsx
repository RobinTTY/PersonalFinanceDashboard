import { ComponentPreview } from '../../../storybook/ComponentPreview';
import { PreferencesSection } from './PreferencesSection';

export default { title: 'Settings/PreferencesSection' };

export function Usage() {
  return (
    <ComponentPreview canvas={{ center: true, maxWidth: 600 }} withSpacing>
      <PreferencesSection />
    </ComponentPreview>
  );
}
