
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 7107

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["MessageQueue/MessageQueue.csproj", "MessageQueue/MessageQueue.csproj"]
COPY ["Contracts/Contracts.csproj", "Contracts/Contracts.csproj"]
COPY ["Common/Common.csproj", "Common/Common.csproj"]
COPY ["ProductService/ProductService.csproj", "ProductService/ProductService.csproj"]

RUN dotnet restore "ProductService/ProductService.csproj"

COPY ["MessageQueue", "MessageQueue"]
COPY ["Contracts", "Contracts"]
COPY ["Common", "Common"]
COPY ["ProductService", "ProductService"]

WORKDIR "/src/."
ARG BUILD_CONFIGURATION=Release
RUN dotnet build "ProductService/ProductService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ProductService/ProductService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
RUN dotnet dev-certs https

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=publish /root/.dotnet/corefx/cryptography/x509stores/my/ /root/.dotnet/corefx/cryptography/x509stores/my/
ENTRYPOINT ["dotnet", "ProductService.dll"]
