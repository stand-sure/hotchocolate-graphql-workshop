ARG VERSION=1.0

FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
ARG VERSION
WORKDIR /src

COPY Data/Data.csproj src/Data/
COPY Models/Models.csproj src/Models/
COPY Service/Service.csproj src/Service/
RUN dotnet restore \
      -p:Version=$VERSION \
    src/Service/Service.csproj

COPY . .
WORKDIR /src/Service
RUN dotnet publish -c Release \
      -o /app \
      -p:Version=$VERSION \
      --self-contained false \
    Service.csproj

FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine AS app

ENV DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE=false
WORKDIR /app
COPY --from=build /app .

ENTRYPOINT ["./ConferencePlannerService"]