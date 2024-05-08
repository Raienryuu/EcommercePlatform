#Set context inside main directory instead of single project directory

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5138

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["OrderService/OrderService.csproj", "OrderService/OrderService.csproj"]
COPY ["MessageQueue/MessageQueue.csproj", "MessageQueue/MessageQueue.csproj"]

RUN dotnet restore "OrderService/OrderService.csproj"

COPY ["OrderService", "OrderService"]
COPY ["MessageQueue", "MessageQueue"]

WORKDIR "/src/."
ARG BUILD_CONFIGURATION=Release
RUN dotnet build "OrderService/OrderService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OrderService/OrderService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderService.dll"]