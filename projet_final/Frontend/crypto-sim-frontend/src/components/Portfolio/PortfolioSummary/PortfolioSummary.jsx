import { getPortfolioSummary } from '../../../services/portfolioService'
import useFetch from '../../../hooks/useFetch'

function PortfolioSummary() {
  const { data: summary, loading, error } = useFetch(getPortfolioSummary)

  if (loading) return <div>Loading...</div>
  if (error) return <div>Error: {error}</div>

  return (
    <div className="portfolio-summary">
      <h2>Portfolio Summary</h2>
      <p>Cash Balance : ${summary.cashBalance}</p>
      <p>Total invested : ${summary.totalInvested}</p>
      <p>Current Value: ${summary.totalCurrentValue}</p>
      <p>Total Gain/Loss: ${summary.totalGainLoss}</p>
      <p>Percent Gain/Loss: {summary.totalGainLossPercent.toFixed(2)}%</p>
      <p>Number of Assets: {summary.holdings.length}</p>
    </div>
  )
}

export default PortfolioSummary
