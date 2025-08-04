# See https://docs.docker.com/engine/reference/builder/
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

ARG AdminUser__DefaultFirstName
ARG AdminUser__DefaultLastName
ARG AdminUser__DefaultUsername
ARG AdminUser__EnableSeeding
ARG AdminUser__UserId
ARG ASPNETCORE_ENVIRONMENT
ARG Auth0__Application__ClientId
ARG Auth0__Application__ClientSecret
ARG Auth0__Audience
ARG Auth0__Domain
ARG Auth0__ManagementApi__ClientId
ARG Auth0__ManagementApi__ClientSecret
ARG Auth0__RedirectUri
ARG ConnectionStrings__PostgresDb
ARG Urls__Http

ENV AdminUser__DefaultFirstName=$AdminUser__DefaultFirstName \
    AdminUser__DefaultLastName=$AdminUser__DefaultLastName \
    AdminUser__DefaultUsername=$AdminUser__DefaultUsername \
    AdminUser__EnableSeeding=$AdminUser__EnableSeeding \
    AdminUser__UserId=$AdminUser__UserId \
    ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT \
    Auth0__Application__ClientId=$Auth0__Application__ClientId \
    Auth0__Application__ClientSecret=$Auth0__Application__ClientSecret \
    Auth0__Audience=$Auth0__Audience \
    Auth0__Domain=$Auth0__Domain \
    Auth0__ManagementApi__ClientId=$Auth0__ManagementApi__ClientId \
    Auth0__ManagementApi__ClientSecret=$Auth0__ManagementApi__ClientSecret \
    Auth0__RedirectUri=$Auth0__RedirectUri \
    ConnectionStrings__PostgresDb=$ConnectionStrings__PostgresDb \
    Urls__Http=$Urls__Http

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "FinanceBackEnd.sln"
RUN dotnet publish "Finance.Api/Finance.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80
ENV HTTP_PORTS=
ENV HTTPS_PORTS=

ENTRYPOINT ["dotnet", "Finance.Api.dll"]
