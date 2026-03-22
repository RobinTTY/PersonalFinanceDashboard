import { CodegenConfig } from "@graphql-codegen/cli";

/**
 * Default codegen configuration for a TypeScript project using Apollo Client.
 * See: https://www.apollographql.com/docs/react/development-testing/graphql-codegen
 */
const config: CodegenConfig = {
  overwrite: true,
  schema: "http://localhost:5115/graphql",
  documents: ["src/**/*.{ts,tsx}"],
  ignoreNoDocuments: true,
  generates: {
    // Use a path that works the best for the structure of your application
    "src/graphql/types/graphql.ts": {
      plugins: ["typescript", "typescript-operations"],
      config: {
        avoidOptionals: {
          // Use `null` for nullable fields instead of optionals
          field: true,
          // Allow nullable input fields to remain unspecified
          inputValue: false,
        },
        // Use `unknown` instead of `any` for unconfigured scalars
        defaultScalarType: "unknown",
        // Always includes `__typename` fields
        nonOptionalTypename: true,
        // Apollo Client doesn't add the `__typename` field to root types so
        // don't generate a type for the `__typename` for root operation types.
        skipTypeNameForRoot: true,
      },
    },
  },
};

export default config;
