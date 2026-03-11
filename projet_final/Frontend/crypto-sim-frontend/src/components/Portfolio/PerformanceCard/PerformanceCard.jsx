import { getPerformanceData } from '../../../services/portfolioService'
import useFetch from '../../../hooks/useFetch'

function PerformanceCard() {
  const { data: performanceData, loading, error } = useFetch(getPerformanceData)

  return (
    <div className="performance-card">
      <h2 className="performance-card__title">Performance du portfolio</h2>
      {loading && <p className="performance-card__message">Loading performance data…</p>}
      {error && <p className="performance-card__error">Error: {error}</p>}
      {!loading && !error && performanceData && (
        <div className="performance-card__data">
          <p>Total investi : ${performanceData.totalInvested}</p>
          <p>Valeur actuelle : ${performanceData.currentValue}</p>
          <p>Gains/Pertes : ${performanceData.gainLoss}</p>
          <p>Pourcentage de gain/perte : {performanceData.gainLossPercent.toFixed(2)}%</p>
        </div>
      )}
    </div>
  )
}

export default PerformanceCard
