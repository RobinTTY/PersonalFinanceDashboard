# Deployment

`docker-compose.app.yml` runs the dashboard in two containers:

- **web**: the client, served by Caddy, which also forwards `/graphql` to the API. The browser talks to a single
  origin, so no CORS configuration is needed.
- **api**: the server. It applies database migrations on startup and keeps the SQLite database in the `data`
  volume.

`docker-compose.yml` only runs Seq, for collecting logs during development.

## Running it

1. Copy `.env.example` to `.env` and fill in your GoCardless secret ID and key.
2. Start the containers:

   ```shell
   docker compose -f docker-compose.app.yml up -d --build
   ```

3. Open http://localhost:8080, or the port set as `PORT` in `.env`.

Run the same command again after pulling changes to rebuild and restart the containers. The database is kept.

## Access

The API has no authentication yet, so the dashboard is only published on `127.0.0.1`. Don't expose it to other
machines until it has. To reach it from elsewhere later, put a reverse proxy in front of it that terminates TLS,
since the containers only serve HTTP.

## Health and logs

- The API reports at `/health` whether it can reach its database. Docker uses this to mark the `api` container
  healthy, and only starts `web` once it is.
- Logs are written to the console: `docker compose -f docker-compose.app.yml logs -f api`. To send them to Seq,
  set `AppConfiguration__LoggingConfiguration__SeqServerUrl` on the `api` service. Logs written while Seq isn't
  reachable are kept in the `data` volume.

## Backups

The database is the file `PersonalFinanceDashboard.db` in the `data` volume (`/data` in the `api` container).
To copy it out:

```shell
docker compose -f docker-compose.app.yml cp api:/data/PersonalFinanceDashboard.db ./backup.db
```

Stop the API first (`docker compose -f docker-compose.app.yml stop api`), so the copy doesn't miss changes that
are still in the database's write-ahead log.
