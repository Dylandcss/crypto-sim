import { getPortfolioSummary } from '../../../services/portfolioService'
import { useState, useEffect } from 'react'
import './PortfolioSummary.Module.css'

function PortfolioSummary() {
  const [summary, setSummary] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)

  useEffect(() => {
    const fetchSummary = async () => {
      try {
        const data = await getPortfolioSummary()
        setSummary(data)
      } catch (err) {
        setError(err.message)
      } finally {
        setLoading(false)
      }
    }
    fetchSummary()
  }, [])

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
