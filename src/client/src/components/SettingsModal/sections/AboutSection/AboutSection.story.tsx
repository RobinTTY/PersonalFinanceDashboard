import { ComponentPreview } from '../../../storybook/ComponentPreview';
import { AboutSection } from './AboutSection';

export default { title: 'Settings/AboutSection' };

export function Usage() {
  return (
    <ComponentPreview canvas={{ center: true, maxWidth: 600 }} withSpacing>
      <AboutSection />
    </ComponentPreview>
  );
}
