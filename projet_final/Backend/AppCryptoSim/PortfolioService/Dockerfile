# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["PortfolioService/PortfolioService.csproj", "PortfolioService/"]
COPY ["CryptoSim.Shared/CryptoSim.Shared.csproj", "CryptoSim.Shared/"]
RUN dotnet restore "PortfolioService/PortfolioService.csproj"

COPY . .
RUN dotnet publish "PortfolioService/PortfolioService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 5003
ENV ASPNETCORE_URLS=http://+:5003

ENTRYPOINT ["dotnet", "PortfolioService.dll"]
