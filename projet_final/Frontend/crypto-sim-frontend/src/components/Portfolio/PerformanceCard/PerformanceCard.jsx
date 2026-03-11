import { getPerformanceData } from '../../../services/portfolioService'
import { useState, useEffect } from 'react'
import './PerformanceCard.Module.css'

function PerformanceCard() {
  const [performanceData, setPerformanceData] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)

  useEffect(() => {
    const fetchPerformanceData = async () => {
      try {
        const data = await getPerformanceData()
        setPerformanceData(data)
      } catch (err) {
        setError(err.message)
      } finally {
        setLoading(false)
      }
    }
    fetchPerformanceData()
  }, [])

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
