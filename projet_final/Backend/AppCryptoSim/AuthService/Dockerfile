# Fichier: `AuthService/Dockerfile`

# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["AuthService/AuthService.csproj", "AuthService/"]
COPY ["CryptoSim.Shared/CryptoSim.Shared.csproj", "CryptoSim.Shared/"]
RUN dotnet restore "AuthService/AuthService.csproj"

COPY . .
RUN dotnet publish "AuthService/AuthService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 5001
ENV ASPNETCORE_URLS=http://+:5001

ENTRYPOINT ["dotnet", "AuthService.dll"]