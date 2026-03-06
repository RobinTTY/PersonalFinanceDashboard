import { render } from '@test-utils';
import * as echarts from 'echarts';
import { NetWorthLineChart } from './NetWorthLineChart';

const chartMock = vi.hoisted(() => ({
  setOption: vi.fn(),
  resize: vi.fn(),
  dispose: vi.fn(),
}));

vi.mock('echarts', () => ({
  init: vi.fn(() => chartMock),
}));

describe('NetWorthLineChart', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('initializes ECharts and applies axis/series options', () => {
    render(
      <NetWorthLineChart
        data={[12400, 12950, 13100, 13820, 14210, 14940]}
        labels={['Oct', 'Nov', 'Dec', 'Jan', 'Feb', 'Mar']}
      />,
    );

    expect(echarts.init).toHaveBeenCalledTimes(1);
    expect(chartMock.setOption).toHaveBeenCalledWith(
      expect.objectContaining({
        xAxis: expect.objectContaining({
          data: ['Oct', 'Nov', 'Dec', 'Jan', 'Feb', 'Mar'],
        }),
        yAxis: expect.objectContaining({
          min: 12000,
          max: 16000,
          interval: 1000,
          splitNumber: 4,
        }),
        series: [
          expect.objectContaining({
            name: 'Combined Accounts',
            data: [12400, 12950, 13100, 13820, 14210, 14940],
            type: 'line',
          }),
        ],
      }),
    );
  });

  it('resizes chart on window resize and disposes on unmount', () => {
    const { unmount } = render(<NetWorthLineChart data={[12000, 13000]} labels={['Jan', 'Feb']} />);

    window.dispatchEvent(new Event('resize'));
    expect(chartMock.resize).toHaveBeenCalledTimes(1);

    unmount();
    expect(chartMock.dispose).toHaveBeenCalledTimes(1);
  });

  it('does not initialize chart when data is empty', () => {
    render(<NetWorthLineChart data={[]} labels={['Jan', 'Feb']} />);
    expect(echarts.init).not.toHaveBeenCalled();
  });
});
