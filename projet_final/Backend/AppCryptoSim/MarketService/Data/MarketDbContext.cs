using MarketService.Models;
using Microsoft.EntityFrameworkCore;

namespace MarketService.Data;

public class MarketDbContext : DbContext
{

    public MarketDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Crypto> Cryptos { get; set; }
    public DbSet<PriceHistory> PriceHistory { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Crypto>().HasData(
            new Crypto { Id = 1, Symbol = "BTC-X", Name = "BitcoinX", CurrentPrice = 42_000m, LastUpdated = DateTime.UtcNow },
            new Crypto { Id = 2, Symbol = "ETH-Z", Name = "EtherZero", CurrentPrice = 2_500m, LastUpdated = DateTime.UtcNow },
            new Crypto { Id = 3, Symbol = "SOL-F", Name = "SolaFake", CurrentPrice = 95m, LastUpdated = DateTime.UtcNow },
            new Crypto { Id = 4, Symbol = "DOG-M", Name = "DogeMoon", CurrentPrice = 0.08m, LastUpdated = DateTime.UtcNow },
            new Crypto { Id = 5, Symbol = "ADA-S", Name = "CardanoSim", CurrentPrice = 0.45m, LastUpdated = DateTime.UtcNow }
        );
    }
}
