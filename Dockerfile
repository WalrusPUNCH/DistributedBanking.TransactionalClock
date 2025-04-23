FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DistributedBanking.TransactionalClock.Data/DistributedBanking.TransactionalClock.Data.csproj", "DistributedBanking.TransactionalClock.Data/"]
COPY ["DistributedBanking.TransactionalClock.Domain/DistributedBanking.TransactionalClock.Domain.csproj", "DistributedBanking.TransactionalClock.Domain/"]
COPY ["DistributedBanking.TransactionalClock.Host/DistributedBanking.TransactionalClock.Host.csproj", "DistributedBanking.TransactionalClock.Host/"]
COPY ["DistributedBanking.Shared/Contracts/Contracts.csproj", "DistributedBanking.Shared/Contracts/"]
COPY ["DistributedBanking.Shared/Shared.Data/Shared.Data.csproj", "DistributedBanking.Shared/Shared.Data/"]
COPY ["DistributedBanking.Shared/Shared.Kafka/Shared.Kafka.csproj", "DistributedBanking.Shared/Shared.Kafka/"]
COPY ["DistributedBanking.Shared/Shared.Messaging/Shared.Messaging.csproj", "DistributedBanking.Shared/Shared.Messaging/"]
COPY ["DistributedBanking.Shared/Shared.Redis/Shared.Redis.csproj", "DistributedBanking.Shared/Shared.Redis/"]
RUN dotnet restore "./DistributedBanking.TransactionalClock.Host/DistributedBanking.TransactionalClock.Host.csproj"
COPY . .
WORKDIR "/src/DistributedBanking.TransactionalClock.Host"
RUN dotnet build "./DistributedBanking.TransactionalClock.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DistributedBanking.TransactionalClock.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DistributedBanking.TransactionalClock.Host.dll"]