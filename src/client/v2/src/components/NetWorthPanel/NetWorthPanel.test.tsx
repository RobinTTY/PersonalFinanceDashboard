import { fireEvent, screen } from '@testing-library/react';
import { render } from '@test-utils';
import { NetWorthPanel } from './NetWorthPanel';
import { computeChange, formatCurrency, RangeKey, sliceRange } from './utility/netWorthRange';

const chartMock = vi.hoisted(() => ({
  setOption: vi.fn(),
  resize: vi.fn(),
  dispose: vi.fn(),
}));

vi.mock('echarts', () => ({
  init: vi.fn(() => chartMock),
  graphic: {
    LinearGradient: class {},
  },
}));

// 120 daily points valued 0..119 so different ranges start from different points.
const data = Object.fromEntries(
  Array.from({ length: 120 }, (_, index) => [`day-${index}`, index])
);

// Mirrors the component's percentage formatting; kept locale-agnostic by reusing Intl.
function expectedPct(range: RangeKey): string {
  const pct = computeChange(sliceRange(data, range)).changePct ?? 0;
  const formatted = new Intl.NumberFormat(undefined, {
    maximumFractionDigits: 1,
    signDisplay: 'exceptZero',
  }).format(pct);
  return `${formatted}%`;
}

describe('NetWorthPanel', () => {
  it('shows the current balance and the change over the default range', () => {
    render(<NetWorthPanel dataByLabel={data} currency="EUR" />);

    // Current balance is the latest point regardless of range.
    expect(screen.getByText(formatCurrency(119, 'EUR'))).toBeInTheDocument();
    // Default range is 3M.
    expect(screen.getByText(expectedPct('3M'))).toBeInTheDocument();
    expect(screen.getByText(/Past 3 months/)).toBeInTheDocument();
  });

  it('recomputes the change when a different range is selected', () => {
    render(<NetWorthPanel dataByLabel={data} currency="EUR" />);

    fireEvent.click(screen.getByText('1M'));

    // The 1M window starts later, so its percentage differs; balance is unchanged.
    expect(screen.getByText(expectedPct('1M'))).toBeInTheDocument();
    expect(screen.getByText(formatCurrency(119, 'EUR'))).toBeInTheDocument();
    expect(screen.getByText(/Past month/)).toBeInTheDocument();
  });
});
