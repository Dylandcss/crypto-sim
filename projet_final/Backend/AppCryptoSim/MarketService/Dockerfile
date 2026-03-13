# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["MarketService/MarketService.csproj", "MarketService/"]
COPY ["CryptoSim.Shared/CryptoSim.Shared.csproj", "CryptoSim.Shared/"]
RUN dotnet restore "MarketService/MarketService.csproj"

COPY . .
RUN dotnet publish "MarketService/MarketService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 5002
ENV ASPNETCORE_URLS=http://+:5002

ENTRYPOINT ["dotnet", "MarketService.dll"]
