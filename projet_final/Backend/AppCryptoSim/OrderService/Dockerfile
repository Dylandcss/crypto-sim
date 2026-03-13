# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["OrderService/OrderService.csproj", "OrderService/"]
COPY ["CryptoSim.Shared/CryptoSim.Shared.csproj", "CryptoSim.Shared/"]
RUN dotnet restore "OrderService/OrderService.csproj"

COPY . .
RUN dotnet publish "OrderService/OrderService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 5004
ENV ASPNETCORE_URLS=http://+:5004

ENTRYPOINT ["dotnet", "OrderService.dll"]
