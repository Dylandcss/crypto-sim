namespace PortfolioService.Dtos;

public record PortfolioPerformanceDto(
    decimal TotalInvested,
    decimal CurrentValue,
    decimal GainLoss,
    decimal GainLossPercent
);