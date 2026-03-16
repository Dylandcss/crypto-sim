using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MarketService.Migrations
{
    /// <inheritdoc />
    public partial class InitialMarketCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Cryptos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Symbol = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CurrentPrice = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cryptos", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PriceHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CryptoSymbol = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Price = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CryptoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceHistory_Cryptos_CryptoId",
                        column: x => x.CryptoId,
                        principalTable: "Cryptos",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Cryptos",
                columns: new[] { "Id", "CurrentPrice", "LastUpdated", "Name", "Symbol" },
                values: new object[,]
                {
                    { 1, 42000m, new DateTime(2026, 3, 8, 0, 15, 53, 26, DateTimeKind.Utc).AddTicks(870), "BitcoinX", "BTC-X" },
                    { 2, 2500m, new DateTime(2026, 3, 8, 0, 15, 53, 26, DateTimeKind.Utc).AddTicks(1127), "EtherZero", "ETH-Z" },
                    { 3, 95m, new DateTime(2026, 3, 8, 0, 15, 53, 26, DateTimeKind.Utc).AddTicks(1129), "SolaFake", "SOL-F" },
                    { 4, 0.08m, new DateTime(2026, 3, 8, 0, 15, 53, 26, DateTimeKind.Utc).AddTicks(1151), "DogeMoon", "DOG-M" },
                    { 5, 0.45m, new DateTime(2026, 3, 8, 0, 15, 53, 26, DateTimeKind.Utc).AddTicks(1189), "CardanoSim", "ADA-S" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PriceHistory_CryptoId",
                table: "PriceHistory",
                column: "CryptoId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceHistory_CryptoSymbol",
                table: "PriceHistory",
                column: "CryptoSymbol");

            migrationBuilder.CreateIndex(
                name: "IX_PriceHistory_RecordedAt",
                table: "PriceHistory",
                column: "RecordedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PriceHistory");

            migrationBuilder.DropTable(
                name: "Cryptos");
        }
    }
}
