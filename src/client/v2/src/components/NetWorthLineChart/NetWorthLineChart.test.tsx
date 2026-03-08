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
        dataByLabel={{
          Oct: 12400,
          Nov: 12950,
          Dec: 13100,
          Jan: 13820,
          Feb: 14210,
          Mar: 14940,
        }}
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
    const { unmount } = render(<NetWorthLineChart dataByLabel={{ Jan: 12000, Feb: 13000 }} />);

    window.dispatchEvent(new Event('resize'));
    expect(chartMock.resize).toHaveBeenCalledTimes(1);

    unmount();
    expect(chartMock.dispose).toHaveBeenCalledTimes(1);
  });

  it('does not initialize chart when data is empty', () => {
    render(<NetWorthLineChart dataByLabel={{}} />);
    expect(echarts.init).not.toHaveBeenCalled();
  });
});
