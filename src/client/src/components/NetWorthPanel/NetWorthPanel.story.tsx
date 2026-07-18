import { ComponentPreview } from '../storybook/ComponentPreview';
import { NetWorthPanel } from './NetWorthPanel';

export default { title: 'Dashboard/NetWorthPanel' };

// A year of gently trending daily points with some noise, so the range switcher
// and change indicator have realistic data to work with.
const sampleData = Object.fromEntries(
  Array.from({ length: 365 }, (_, index) => {
    const date = new Date(2025, 6, 12);
    date.setDate(date.getDate() + index);
    const label = date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    });
    const trend = 8000 + index * 12;
    const wobble = Math.sin(index / 9) * 600 + Math.sin(index / 2) * 120;
    return [label, Math.round(trend + wobble)];
  })
);

export function Usage() {
  return (
    <ComponentPreview canvas={{ center: true, maxWidth: 720 }} withSpacing>
      <NetWorthPanel dataByLabel={sampleData} currency="EUR" />
    </ComponentPreview>
  );
}
