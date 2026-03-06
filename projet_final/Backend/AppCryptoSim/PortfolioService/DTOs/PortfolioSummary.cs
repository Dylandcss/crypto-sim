namespace PortfolioService.DTOs
{
    public record PortfolioSummary(
     int UserId,
     decimal CashBalance,
     decimal TotalInvested,
     decimal TotalCurrentValue,
     decimal TotalGainLoss,
     decimal TotalGainLossPercent,
     List<HoldingDetail> Holdings
 );
}
