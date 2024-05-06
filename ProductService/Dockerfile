#Set context inside main directory instead of single project directory

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 7107

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ProductService/ProductService.csproj", "ProductService/ProductService.csproj"]
COPY ["MessageQueue/MessageQueue.csproj", "MessageQueue/MessageQueue.csproj"]

RUN dotnet restore "ProductService/ProductService.csproj"

COPY ["ProductService", "ProductService"]
COPY ["MessageQueue", "MessageQueue"]

WORKDIR "/src/."
RUN dotnet build "ProductService/ProductService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProductService/ProductService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductService.dll"]