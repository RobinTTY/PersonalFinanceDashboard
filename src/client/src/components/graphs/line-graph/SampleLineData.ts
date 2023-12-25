import { EChartProps } from '@components/graphs/line-graph/EChartProps';

const endDate = new Date();
const netWorth = [3360, 4241, 5816, 7115, 7567, 8374, 9564, 11835, 11532, 12315, 13147, 13421];

const getPortfolioValue = (index: number) =>
  netWorth[index] - Math.floor(Math.random() * 1000) - Math.floor(Math.random() * 1500);
const portfolioValue = [
  getPortfolioValue(0),
  getPortfolioValue(1),
  getPortfolioValue(2),
  getPortfolioValue(3),
  getPortfolioValue(4),
  getPortfolioValue(5),
  getPortfolioValue(6),
  getPortfolioValue(7),
  getPortfolioValue(8),
  getPortfolioValue(9),
  getPortfolioValue(10),
  getPortfolioValue(11),
];

const dates = [
  new Date(endDate.getTime() - 12 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
  new Date(endDate.getTime() - 11 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
  new Date(endDate.getTime() - 10 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
  new Date(endDate.getTime() - 9 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
  new Date(endDate.getTime() - 8 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
  new Date(endDate.getTime() - 7 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
  new Date(endDate.getTime() - 6 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
  new Date(endDate.getTime() - 5 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
  new Date(endDate.getTime() - 4 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
  new Date(endDate.getTime() - 3 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
  new Date(endDate.getTime() - 2 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
  new Date(endDate.getTime() - 1 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
];

export const SampleLineData: EChartProps['option'] = {
  tooltip: {
    trigger: 'axis',
  },
  // legend: {
  //   bottom: 10,
  //   left: 'center',
  //   data: ['Net Worth', 'Portfolio Value'],
  // },
  xAxis: {
    type: 'category',
    data: dates,
  },
  yAxis: {
    type: 'value',
  },
  series: [
    {
      data: [3360, 4241, 5816, 7115, 7567, 8374, 9564, 11835, 11532, 12315, 13147, 13421],
      type: 'line',
    },
    {
      data: portfolioValue,
      type: 'line',
    },
  ],
};
