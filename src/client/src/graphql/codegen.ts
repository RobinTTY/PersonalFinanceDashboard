import { CodegenConfig } from '@graphql-codegen/cli';
import { loadEnv } from 'vite';

// Same env files as the app, so VITE_API_URL can be overridden in .env.local or the environment
const env = loadEnv('development', 'env-settings', 'VITE_');

/**
 * Default codegen configuration for a TypeScript project using Apollo Client.
 * See: https://www.apollographql.com/docs/react/development-testing/graphql-codegen
 */
const config: CodegenConfig = {
  overwrite: true,
  schema: env.VITE_API_URL,
  documents: ['src/**/*.{ts,tsx}'],
  ignoreNoDocuments: true,
  hooks: { afterAllFileWrite: ['prettier --write'] },
  generates: {
    'src/graphql/types/graphql.ts': {
      plugins: ['typescript', 'typescript-operations'],
      config: {
        avoidOptionals: {
          /**
           * Use `null` for nullable fields instead of optionals
           */
          field: true,
          /**
           * Allow nullable input fields to remain unspecified
           */
          inputValue: false,
        },
        /**
         * Use `unknown` instead of `any` for non-configured scalars
         */
        defaultScalarType: 'unknown',
        /**
         * Always includes `__typename` fields
         */
        nonOptionalTypename: true,
        /**
         * Apollo Client doesn't add the `__typename` field to root types so
         * don't generate a type for the `__typename` for root operation types.
         */
        skipTypeNameForRoot: true,
        /**
         * Map custom scalars to their TypeScript runtime equivalents
         */
        scalars: {
          DateTime: 'Date',
          Decimal: 'number',
        },
      },
    },
    'src/graphql/types/type-policies.ts': {
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
};

export default config;
