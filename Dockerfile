# syntax = docker/dockerfile:1.2
ARG VERSION=1.0

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG VERSION
WORKDIR /src

COPY Data/Data.csproj src/Data/
COPY Models/Models.csproj src/Models/
COPY Service/Service.csproj src/Service/
RUN --mount=type=cache,target=/var/root/.nuget/packages \
    --mount=type=cache,target=/var/root/.local/share/NuGet/v3-cache \
    dotnet restore \
      -p:Version=$VERSION \
    src/Service/Service.csproj

COPY . .
WORKDIR /src/Service
RUN --mount=type=cache,target=/var/root/.nuget/packages \
    --mount=type=cache,target=/var/root/.local/share/NuGet/v3-cache \
    dotnet publish -c Release \
      -o /app \
      -p:Version=$VERSION \
      --self-contained false \
    Service.csproj


RUN apt update && apt -y upgrade

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS app

WORKDIR /app
COPY --from=build /app .

ENTRYPOINT ["./ConferencePlannerService"]