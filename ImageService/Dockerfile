
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5271

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ImageService/ImageService.csproj", "ImageService/ImageService.csproj"]
COPY ["MessageQueue/MessageQueue.csproj", "MessageQueue/MessageQueue.csproj"]
COPY ["Common/Common.csproj", "Common/Common.csproj"]

RUN dotnet restore "ImageService/ImageService.csproj"

COPY ["ImageService", "ImageService"]
COPY ["Common", "Common"]
COPY ["MessageQueue", "MessageQueue"]

WORKDIR "/src/."
ARG BUILD_CONFIGURATION=Release
RUN dotnet build "ImageService/ImageService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ImageService/ImageService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
RUN dotnet dev-certs https

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=publish /root/.dotnet/corefx/cryptography/x509stores/my/ /root/.dotnet/corefx/cryptography/x509stores/my/
ENTRYPOINT ["dotnet", "ImageService.dll"]
