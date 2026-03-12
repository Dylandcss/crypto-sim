import { getPortfolioSummary } from '../../../services/portfolioService'
import useFetch from '../../../hooks/useFetch'

function PortfolioSummary() {
  const { data: summary, loading, error } = useFetch(getPortfolioSummary)

  if (loading) return <div>Loading...</div>
  if (error) return <div>Error: {error}</div>

  return (
    <div className="portfolio-summary">
      <h2>Résumé du portefeuille</h2>
      <p>Argent disponible : ${summary.cashBalance}</p>
      <p>Total investi : ${summary.totalInvested}</p>
      <p>Valeur actuelle: ${summary.totalCurrentValue}</p>
      <p>Total Gain/Perte: ${summary.totalGainLoss}</p>
      <p>Pourcentage Gain/Perte: {summary.totalGainLossPercent.toFixed(2)}%</p>
      <p>Nombre d'actifs: {summary.holdings.length}</p>
    </div>
  )
}

export default PortfolioSummary
