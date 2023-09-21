import { Text, Paper, useMantineTheme } from '@mantine/core';
import { ResponsiveLine } from '@nivo/line';
import { SampleLineData as data } from './SampleLineData';
import { NivoGridWrapper } from '../nivo-grid-wrapper/NivoGridWrapper';
import './LineGraph.css';

// TODO: typing for data
// TODO: theme color switch (light mode)
export const LineGraph = () => {
  const theme = useMantineTheme();

  return (
    <Paper withBorder p="md" radius="md" h="50%">
      <div id="main-container">
        <Text size="lg" pl="xl" c="white">
          Net Worth
        </Text>
        <NivoGridWrapper>
          <ResponsiveLine
            data={data}
            theme={{
              axis: {
                domain: {
                  line: {
                    stroke: theme.colors.gray[1],
                  },
                },
                legend: {
                  text: {
                    fill: theme.colors.gray[1],
                  },
                },
                ticks: {
                  line: {
                    stroke: theme.colors.gray[1],
                    strokeWidth: 1,
                  },
                  text: {
                    fill: theme.colors.gray[1],
                  },
                },
              },
              legends: {
                text: {
                  fill: theme.colors.gray[1],
                },
              },
              tooltip: {
                container: {
                  color: theme.colors.dark[4],
                },
              },
              crosshair: {
                line: {
                  stroke: '#afffff',
                  strokeWidth: 1.5,
                  strokeOpacity: 0.75,
                },
              },
            }}
            margin={{ top: 20, right: 110, bottom: 40, left: 60 }}
            xScale={{ type: 'point' }}
            yScale={{
              type: 'linear',
              min: 'auto',
              max: 'auto',
              stacked: false,
              reverse: false,
            }}
            yFormat=" >-.2f"
            axisTop={null}
            axisRight={null}
            axisLeft={{
              tickSize: 5,
              tickPadding: 5,
              tickRotation: 0,
              legend: 'USD',
              legendOffset: -50,
              legendPosition: 'middle',
            }}
            pointSize={10}
            pointColor={{ theme: 'background' }}
            pointBorderWidth={2}
            pointBorderColor={{ from: 'serieColor' }}
            pointLabelYOffset={-12}
            useMesh={true}
            legends={[
              {
                anchor: 'bottom-right',
                direction: 'column',
                justify: false,
                translateX: 100,
                translateY: 0,
                itemsSpacing: 0,
                itemDirection: 'left-to-right',
                itemWidth: 80,
                itemHeight: 20,
                itemOpacity: 0.75,
                symbolSize: 12,
                symbolShape: 'circle',
                symbolBorderColor: 'rgba(0, 0, 0, .5)',
                effects: [
                  {
                    on: 'hover',
                    style: {
                      itemBackground: 'rgba(0, 0, 0, .03)',
                      itemOpacity: 1,
                    },
                  },
                ],
              },
            ]}
          />
        </NivoGridWrapper>
      </div>
    </Paper>
  );
};
