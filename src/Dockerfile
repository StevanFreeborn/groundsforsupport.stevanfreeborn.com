FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

RUN apt-get update && \
    apt-get install -y curl && \
    curl -fsSL https://deb.nodesource.com/setup_current.x | bash - && \
    apt-get install -y nodejs

COPY ["GroundsForSupport.Server/GroundsForSupport.Server.csproj", "GroundsForSupport.Server/"]
RUN dotnet restore "GroundsForSupport.Server/GroundsForSupport.Server.csproj"

COPY . .
WORKDIR "/src/GroundsForSupport.Server"
RUN dotnet build "GroundsForSupport.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GroundsForSupport.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GroundsForSupport.Server.dll"]
