import { CodegenConfig } from '@graphql-codegen/cli';

const config: CodegenConfig = {
  // Should point to the graphql server
  schema: 'http://192.168.178.10:5115/graphql',
  documents: ['src/**/*.{ts,tsx}'],
  generates: {
    './src/graphql-types/': {
      preset: 'client',
      plugins: [],
      presetConfig: {
        gqlTagName: 'gql',
      },
    },
  },
  ignoreNoDocuments: true,
};

export default config;
