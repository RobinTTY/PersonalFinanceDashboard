import { computeChange, sliceRange } from './netWorthRange';

/** Builds a daily series of `count` sequential values keyed by an index label. */
function series(values: number[]): Record<string, number> {
  return Object.fromEntries(values.map((value, index) => [`day-${index}`, value]));
}

describe('sliceRange', () => {
  it('keeps only the trailing days for the range', () => {
    const data = series(Array.from({ length: 400 }, (_, index) => index));

    expect(Object.keys(sliceRange(data, '1M'))).toHaveLength(30);
    expect(Object.keys(sliceRange(data, '3M'))).toHaveLength(90);
    expect(Object.keys(sliceRange(data, '6M'))).toHaveLength(180);
    expect(Object.keys(sliceRange(data, '1Y'))).toHaveLength(365);
  });

  it('returns the whole series when fewer days exist than the range', () => {
    const data = series([1, 2, 3]);
    expect(sliceRange(data, '1Y')).toEqual(data);
  });

  it('slices from the most recent end', () => {
    const data = series([10, 20, 30, 40]);
    const sliced = sliceRange(data, '1M');
    // 1M keeps 30 days; with only 4 present the last value must still be the newest.
    expect(Object.values(sliced).at(-1)).toBe(40);
  });
});

describe('computeChange', () => {
  it('reports the current value, absolute change, and percentage', () => {
    const result = computeChange(series([100, 110, 125]));
    expect(result.current).toBe(125);
    expect(result.change).toBe(25);
    expect(result.changePct).toBe(25);
    expect(result.direction).toBe('up');
  });

  it('marks a decline as down with a negative change', () => {
    const result = computeChange(series([200, 150]));
    expect(result.change).toBe(-50);
    expect(result.changePct).toBe(-25);
    expect(result.direction).toBe('down');
  });

  it('is flat when the endpoints are equal', () => {
    const result = computeChange(series([100, 130, 100]));
    expect(result.change).toBe(0);
    expect(result.direction).toBe('flat');
  });

  it('uses the absolute starting value so the percentage sign follows the change', () => {
    const result = computeChange(series([-100, -50]));
    expect(result.change).toBe(50);
    expect(result.changePct).toBe(50); // +50 relative to |−100|
    expect(result.direction).toBe('up');
  });

  it('returns a null percentage when the first value is zero', () => {
    const result = computeChange(series([0, 40]));
    expect(result.change).toBe(40);
    expect(result.changePct).toBeNull();
  });

  it('handles an empty series', () => {
    expect(computeChange({})).toEqual({
      current: 0,
      change: 0,
      changePct: null,
      direction: 'flat',
    });
  });
});
