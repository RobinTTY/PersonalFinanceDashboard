import { Loader, createTheme } from '@mantine/core';

export const theme = createTheme({
  components: {
    Loader: Loader.extend({
      defaultProps: {
        type: 'bars',
      },
    }),
  },
});
