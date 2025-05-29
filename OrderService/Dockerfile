
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5138

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MessageQueue/MessageQueue.csproj", "MessageQueue/MessageQueue.csproj"]
COPY ["Contracts/Contracts.csproj", "Contracts/Contracts.csproj"]
COPY ["OrderService/OrderService.csproj", "OrderService/OrderService.csproj"]

RUN dotnet restore "OrderService/OrderService.csproj"

COPY ["MessageQueue", "MessageQueue"]
COPY ["Contracts", "Contracts"]
COPY ["OrderService", "OrderService"]

WORKDIR "/src/."
ARG BUILD_CONFIGURATION=Release
RUN dotnet build "OrderService/OrderService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OrderService/OrderService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
RUN dotnet dev-certs https

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=publish /root/.dotnet/corefx/cryptography/x509stores/my/ /root/.dotnet/corefx/cryptography/x509stores/my/
ENTRYPOINT ["dotnet", "OrderService.dll"]
