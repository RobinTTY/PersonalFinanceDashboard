import * as path from 'path';
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import tsconfigPaths from 'vite-tsconfig-paths';
import checker from 'vite-plugin-checker';

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    react(),
    tsconfigPaths(),
    checker({ typescript: true, eslint: { lintCommand: 'eslint src', useFlatConfig: true } }),
  ],
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: './vitest.setup.mjs',
  },
  resolve: {
    alias: [
      { find: '@', replacement: path.resolve(__dirname, 'src') },
      { find: '@components', replacement: path.resolve(__dirname, 'src/components') },
      { find: '@modals', replacement: path.resolve(__dirname, 'src/modals') },
      { find: '@graphql-queries', replacement: path.resolve(__dirname, 'src/graphql/queries') },
      { find: '@graphql-mutations', replacement: path.resolve(__dirname, 'src/graphql/mutations') },
      { find: '@graphql-types', replacement: path.resolve(__dirname, 'src/graphql/types') },
    ],
  },
});
