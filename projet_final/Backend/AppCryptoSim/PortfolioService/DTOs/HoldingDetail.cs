namespace PortfolioService.Dtos
{
    public record HoldingDetail(
    string Symbol,
    string Name,
    decimal Quantity,
    decimal AverageBuyPrice,
    decimal CurrentPrice,
    decimal CurrentValue,
    decimal GainLoss,
    decimal GainLossPercent
);

}
