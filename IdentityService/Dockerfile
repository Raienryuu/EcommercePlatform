#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["IdentityService/IdentityService.csproj", "."]

RUN dotnet restore "IdentityService.csproj"
COPY ["IdentityService", "IdentityService"]

WORKDIR "/src/."
RUN dotnet build "IdentityService/IdentityService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IdentityService/IdentityService.csproj" -c Release -o /app/publish /p:UseAppHost=false
RUN dotnet dev-certs https

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=publish /root/.dotnet/corefx/cryptography/x509stores/my/ /root/.dotnet/corefx/cryptography/x509stores/my/
ENTRYPOINT ["dotnet", "IdentityService.dll"]