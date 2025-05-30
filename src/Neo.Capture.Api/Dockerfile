# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine3.20 AS base

USER root

RUN apk update && apk add --no-cache icu-libs libssl3 tzdata


USER app
WORKDIR /app
EXPOSE 80
EXPOSE 443


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine3.20 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src


COPY ["src/Neo.Capture.Api/Neo.Capture.Api.csproj", "src/Neo.Capture.Api/"]
COPY ["src/Neo.Capture.Infrastructure/Neo.Capture.Infrastructure.csproj", "src/Neo.Capture.Infrastructure/"]
COPY ["src/Neo.Capture.Application/Neo.Capture.Application.csproj", "src/Neo.Capture.Application/"]
COPY ["src/Neo.Capture.Domain/Neo.Capture.Domain.csproj", "src/Neo.Capture.Domain/"]

ARG username  
ARG password
RUN dotnet nuget add source "https://pkgs.dev.azure.com/cmeoffshore/bb7e9830-d551-48bb-ac0c-d6d60dd4e941/_packaging/LowCode-Hub/nuget/v3/index.json" -n LowCodeHub --store-password-in-clear-text -u $username -p $password
RUN dotnet nuget add source "https://pkgs.dev.azure.com/cmeoffshore/Neo/_packaging/Neo-Common/nuget/v3/index.json" -n Neo-Common --store-password-in-clear-text -u $username -p $password


RUN dotnet restore "./src/Neo.Capture.Api/Neo.Capture.Api.csproj"

COPY . .
WORKDIR "/src/src/Neo.Capture.Api"
RUN dotnet build "./Neo.Capture.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Neo.Capture.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)

# Final stage for running the application in production
FROM base AS final
WORKDIR /app

# Ensure DOTNET uses ICU for globalization
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

USER root
RUN chmod -R 777 /app
# Switch back to app user
USER app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Neo.Capture.Api.dll"]
