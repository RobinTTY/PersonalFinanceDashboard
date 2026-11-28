# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restored on their own, so the packages stay cached until a project file changes
COPY Directory.Build.props Directory.Packages.props ./
COPY RobinTTY.PersonalFinanceDashboard.API/*.csproj RobinTTY.PersonalFinanceDashboard.API/
COPY RobinTTY.PersonalFinanceDashboard.Core/*.csproj RobinTTY.PersonalFinanceDashboard.Core/
COPY RobinTTY.PersonalFinanceDashboard.Infrastructure/*.csproj RobinTTY.PersonalFinanceDashboard.Infrastructure/
COPY RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders/*.csproj RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders/
RUN dotnet restore RobinTTY.PersonalFinanceDashboard.API/RobinTTY.PersonalFinanceDashboard.API.csproj

COPY . .
RUN dotnet publish RobinTTY.PersonalFinanceDashboard.API/RobinTTY.PersonalFinanceDashboard.API.csproj \
    --configuration Release --no-restore --output /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
# curl is only used by the health check
RUN apt-get update \
    && apt-get install --yes --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

# Holds the database and the log files written while Seq isn't reachable, mount a volume here
RUN mkdir /data && chown "$APP_UID" /data
VOLUME /data

WORKDIR /app
COPY --from=build /app .

ENV AppConfiguration__DatabaseConfiguration__ConnectionString="Data Source=/data/PersonalFinanceDashboard.db" \
    AppConfiguration__LoggingConfiguration__SeqFallbackPath="/data/logs/log-.clef"
EXPOSE 8080
USER $APP_UID

HEALTHCHECK --interval=30s --timeout=5s --start-period=30s --retries=3 \
    CMD curl --fail --silent http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "RobinTTY.PersonalFinanceDashboard.Api.dll"]
