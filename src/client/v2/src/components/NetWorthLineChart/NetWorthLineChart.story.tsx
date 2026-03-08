import { ComponentPreview } from '../storybook/ComponentPreview';
import { NetWorthLineChart } from './NetWorthLineChart';

export default { title: 'Dashboard/NetWorthLineChart' };

export function Usage() {
  return (
    <ComponentPreview canvas={{ center: true, maxWidth: 720 }} withSpacing>
      <NetWorthLineChart
        dataByLabel={{
          Oct: 12400,
          Nov: 12950,
          Dec: 13100,
          Jan: 13820,
          Feb: 14210,
          Mar: 14940,
        }}
      />
    </ComponentPreview>
  );
}
