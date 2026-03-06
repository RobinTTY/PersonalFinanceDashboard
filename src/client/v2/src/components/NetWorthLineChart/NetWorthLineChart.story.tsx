import { ComponentPreview } from '../storybook/ComponentPreview';
import { NetWorthLineChart } from './NetWorthLineChart';

export default { title: 'Dashboard/NetWorthLineChart' };

export function Usage() {
  return (
    <ComponentPreview canvas={{ center: true, maxWidth: 720 }} withSpacing>
      <NetWorthLineChart
        data={[12400, 12950, 13100, 13820, 14210, 14940]}
        labels={['Oct', 'Nov', 'Dec', 'Jan', 'Feb', 'Mar']}
      />
    </ComponentPreview>
  );
}
