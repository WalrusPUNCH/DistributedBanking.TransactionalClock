FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DistributedBanking.TransactionalClock.Host/DistributedBanking.TransactionalClock.Host.csproj", "DistributedBanking.TransactionalClock.Host/"]
RUN dotnet restore "DistributedBanking.TransactionalClock.Host/DistributedBanking.TransactionalClock.Host.csproj"
COPY . .
WORKDIR "/src/DistributedBanking.TransactionalClock.Host"
RUN dotnet build "DistributedBanking.TransactionalClock.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DistributedBanking.TransactionalClock.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DistributedBanking.TransactionalClock.Host.dll"]
