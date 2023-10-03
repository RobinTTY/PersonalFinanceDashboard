import { CodegenConfig } from '@graphql-codegen/cli';

const config: CodegenConfig = {
  // Should point to the graphql server
  schema: 'http://192.168.178.10:5115/graphql',
  documents: ['src/**/*.{ts,tsx}'],
  generates: {
    './src/graphql/types/': {
      preset: 'client',
      plugins: [],
      presetConfig: {
        gqlTagName: 'gql',
      },
      config: {
        scalars: {
          DateTime: 'Date',
          Decimal: 'number',
        },
      },
    },
    './src/graphql/types/type-policies.ts': {
      plugins: ['@homebound/graphql-typescript-scalar-type-policies'],
      config: {
        scalars: {
          DateTime: 'Date',
        },
        scalarTypePolicies: {
          DateTime: '../type-policies/DateTime.js#dateTimeTypePolicy',
        },
      },
    },
  },
  ignoreNoDocuments: true,
};

export default config;
