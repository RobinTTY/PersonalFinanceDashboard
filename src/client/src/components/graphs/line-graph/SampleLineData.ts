import { EChartProps } from '@components/graphs/line-graph/EChartProps';

export const echartOptions: EChartProps["option"] = {
  xAxis: {
    type: 'category',
    data: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']
  },
  yAxis: {
    type: 'value'
  },
  series: [
    {
      data: [150, 230, 224, 218, 135, 147, 260],
      type: 'line'
    },
  ],
};

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

export const SampleLineData = [
  {
    id: 'Net Worth',
    color: 'hsl(289, 70%, 50%)',
    data: [
      {
        x: new Date(endDate.getTime() - 12 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
        y: netWorth[0],
      },
      {
        x: new Date(endDate.getTime() - 11 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
        y: netWorth[1],
      },
      {
        x: new Date(endDate.getTime() - 10 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
        y: netWorth[2],
      },
      {
        x: new Date(endDate.getTime() - 9 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
        y: netWorth[3],
      },
      {
        x: new Date(endDate.getTime() - 8 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
        y: netWorth[4],
      },
      {
        x: new Date(endDate.getTime() - 7 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
        y: netWorth[5],
      },
      {
        x: new Date(endDate.getTime() - 6 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
        y: netWorth[6],
      },
      {
        x: new Date(endDate.getTime() - 5 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
        y: netWorth[7],
      },
      {
        x: new Date(endDate.getTime() - 4 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
        y: netWorth[8],
      },
      {
        x: new Date(endDate.getTime() - 3 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
        y: netWorth[9],
      },
      {
        x: new Date(endDate.getTime() - 2 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
        y: netWorth[10],
      },
      {
        x: new Date(endDate.getTime() - 1 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
        y: netWorth[11],
      },
    ],
  },
  {
    id: 'Stocks',
    color: 'hsl(306, 70%, 50%)',
    data: [
      {
        x: new Date(endDate.getTime() - 12 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
        y: portfolioValue[0],
      },
      {
        x: new Date(endDate.getTime() - 11 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
        y: portfolioValue[1],
      },
      {
        x: new Date(endDate.getTime() - 10 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
        y: portfolioValue[2],
      },
      {
        x: new Date(endDate.getTime() - 9 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
        y: portfolioValue[3],
      },
      {
        x: new Date(endDate.getTime() - 8 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
        y: portfolioValue[4],
      },
      {
        x: new Date(endDate.getTime() - 7 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
        y: portfolioValue[5],
      },
      {
        x: new Date(endDate.getTime() - 6 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
        y: portfolioValue[6],
      },
      {
        x: new Date(endDate.getTime() - 5 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
        y: portfolioValue[7],
      },
      {
        x: new Date(endDate.getTime() - 4 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
        y: portfolioValue[8],
      },
      {
        x: new Date(endDate.getTime() - 3 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
        y: portfolioValue[9],
      },
      {
        x: new Date(endDate.getTime() - 2 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
        y: portfolioValue[10],
      },
      {
        x: new Date(endDate.getTime() - 1 * 24 * 60 * 60 * 1000).toLocaleDateString('de'),
        y: portfolioValue[11],
      },
    ],
  },
];
