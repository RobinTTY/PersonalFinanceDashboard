# Contributing

The [README](./README.md) explains how to set up and run the project. These are the conventions of the code base.

## Commits and pull requests

- Commit messages follow [Conventional Commits](https://www.conventionalcommits.org/), with the part as scope when
  a change is limited to it: `feat(client): ...`, `fix(server): ...`, `refactor: ...`, `chore: ...`.
- A pull request passes the checks listed in the README, which CI runs as well.

## Comments

- Comment sparingly and keep comments short. Explain why the code does something when it isn't obvious, not what
  it does.
- Comments describe the code as it is, not how it got there. The history of a fix belongs in the commit message.
- Public server members have XML docs (`/// <summary>`), implementations use `/// <inheritdoc />`.

## Server

- Formatting follows the JetBrains "Reformat Code" profile and `.editorconfig`. Run the formatter command from the
  README, or enable the pre-push hook.
- Release builds treat warnings as errors.
- Use the injected `TimeProvider` instead of `DateTime.UtcNow`, so tests control the clock.
- Pass the `CancellationToken` of a request or synchronization down to the database and data provider calls.
- After changing the model, add a migration and check that EF reports no pending model changes:

  ```shell
  dotnet ef migrations add <Name> --project RobinTTY.PersonalFinanceDashboard.Infrastructure --startup-project RobinTTY.PersonalFinanceDashboard.API
  ```

  Migrations that change data as well as the schema need a look at the generated SQL. SQLite rebuilds tables
  at the end of a migration, after any SQL in it has run.
- Tests use xunit and are named `Scenario_ExpectedOutcome`. Sync handler tests run against an in-memory SQLite
  database with a fake GoCardless API (`SyncTestEnvironment`), API tests against the whole API (`ApiFactory`).

## Client

- Formatting follows Prettier, `npm run prettier:write` applies it.
- Style Mantine components through their `styles` (or `classNames`) prop for their inner parts, instead of
  wrapping content in extra styled elements.
- The types in `src/graphql/types` are generated, don't edit them by hand. Regenerate them after changing a query
  or the server's schema.
