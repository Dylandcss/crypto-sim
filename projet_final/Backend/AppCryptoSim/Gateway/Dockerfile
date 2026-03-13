# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Gateway/Gateway.csproj", "Gateway/"]
COPY ["CryptoSim.Shared/CryptoSim.Shared.csproj", "CryptoSim.Shared/"]
RUN dotnet restore "Gateway/Gateway.csproj"

COPY . .
RUN dotnet publish "Gateway/Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "Gateway.dll"]
