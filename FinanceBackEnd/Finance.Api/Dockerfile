# See https://docs.docker.com/engine/reference/builder/
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
ARG PORT=5000
EXPOSE $PORT

ARG AdminUser__DefaultFirstName
ARG AdminUser__DefaultLastName
ARG AdminUser__DefaultUsername
ARG AdminUser__EnableSeeding
ARG AdminUser__UserId
ARG ASPNETCORE_ENVIRONMENT
ARG Auth0__Application__ClientId
ARG Auth0__Audience
ARG Auth0__Domain
ARG Auth0__ManagementApi__ClientId
ARG Auth0__RedirectUri
ARG Urls__Http

# WARNING: Do NOT include secrets (client secrets, management API secrets, connection strings)
# as build-time ARGs or bake them into image ENV. Secrets should be provided at runtime using
# your deployment platform (Kubernetes Secrets, environment variables injected by the orchestrator,
# or secure secret stores). The following sensitive values were intentionally removed from the
# Dockerfile to avoid image history leakage: Auth0__Application__ClientSecret,
# Auth0__ManagementApi__ClientSecret, ConnectionStrings__PostgresDb.

ENV AdminUser__DefaultFirstName=$AdminUser__DefaultFirstName \
    AdminUser__DefaultLastName=$AdminUser__DefaultLastName \
    AdminUser__DefaultUsername=$AdminUser__DefaultUsername \
    AdminUser__EnableSeeding=$AdminUser__EnableSeeding \
    AdminUser__UserId=$AdminUser__UserId \
    ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT \
    Auth0__Application__ClientId=$Auth0__Application__ClientId \
    Auth0__Audience=$Auth0__Audience \
    Auth0__Domain=$Auth0__Domain \
    Auth0__ManagementApi__ClientId=$Auth0__ManagementApi__ClientId \
    Auth0__RedirectUri=$Auth0__RedirectUri \
    Urls__Http=$Urls__Http \
    Auth0__Application__ClientSecret="" \
    Auth0__ManagementApi__ClientSecret="" \
    ConnectionStrings__PostgresDb=""

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "FinanceBackEnd.sln"
RUN dotnet publish "Finance.Api/Finance.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# ASP.NET Core will automatically use PORT environment variable if ASPNETCORE_URLS is not set
ENV PORT=${PORT:-5000}
ENV HTTP_PORTS=
ENV HTTPS_PORTS=

ENTRYPOINT ["dotnet", "Finance.Api.dll"]
