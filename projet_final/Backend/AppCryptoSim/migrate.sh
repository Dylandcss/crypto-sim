#!/bin/bash
set -Eeuo pipefail

echo "=== efmigrator start ==="

# Stopper immédiatement en cas d'erreur pour que compose détecte l'échec.
trap 'echo "Migration failure at line $LINENO" >&2' ERR

echo "Waiting for MySQL..."
until mysql -hmysql -uroot -pformation -e 'SELECT 1' > /dev/null 2>&1; do
  sleep 2
done

echo "Creating databases..."
mysql -hmysql -uroot -pformation -e "
CREATE DATABASE IF NOT EXISTS cs_users_db;
CREATE DATABASE IF NOT EXISTS cs_market_db;
CREATE DATABASE IF NOT EXISTS cs_orders_db;
CREATE DATABASE IF NOT EXISTS cs_portfolio_db;"

run_migration() {
  local service="$1"
  local project_path="$2"

  echo "Restoring packages for ${service}..."
  dotnet restore "${project_path}" --force

  echo "Applying migrations for ${service}..."
  dotnet-ef database update --project "${project_path}" --startup-project "${project_path}" --verbose
}

echo "Running migrations..."
run_migration "AuthService" "AuthService/AuthService.csproj"
run_migration "MarketService" "MarketService/MarketService.csproj"
run_migration "OrderService" "OrderService/OrderService.csproj"
run_migration "PortfolioService" "PortfolioService/PortfolioService.csproj"

echo "=== efmigrator done ==="