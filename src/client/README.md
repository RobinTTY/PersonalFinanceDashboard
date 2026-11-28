# Client

The dashboard's web client, built with [React](https://react.dev/), [Mantine](https://mantine.dev/) and
[Apollo Client](https://www.apollographql.com/docs/react/). See the [root README](../../README.md) for how to set it
up together with the server.

## Configuration

Settings are read from `env-settings/.env`. Override them in `env-settings/.env.local`, which isn't committed:

- `VITE_API_URL`: the GraphQL endpoint of the server. A relative URL (e.g. `/graphql`) is resolved against the page.
- `VITE_ENABLE_API_MOCKING`: `true` serves the responses in `src/mocks` instead of calling the server.

## Scripts

### Development

- `dev`: starts the development server
- `build`: type-checks and builds the production version to `dist`
- `preview`: serves the production build locally
- `graphql-schema-export`: exports the server's schema to `src/graphql/schema.graphql`, after building the server
- `graphql-types-generate`: regenerates the types in `src/graphql/types` from that schema and the queries

### Checks

- `test`: runs the tests with [Vitest](https://vitest.dev/), `test:watch` keeps running them on changes
- `typecheck`: checks the TypeScript types
- `lint`: runs ESLint (`eslint`) and Stylelint (`stylelint`)
- `prettier`: checks the formatting, `prettier:write` fixes it

### Storybook

- `storybook`: starts [Storybook](https://storybook.js.org/) at http://localhost:6006
- `storybook:build`: builds the stories to `storybook-static`
