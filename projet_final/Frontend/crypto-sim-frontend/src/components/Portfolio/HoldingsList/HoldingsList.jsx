import { getHoldings } from '../../../services/portfolioService'
import { useState, useEffect } from 'react'
import './HoldingsList.Module.css'
import { Link } from 'react-router-dom'

function HoldingsList() {
  const [holdings, setHoldings] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)

  useEffect(() => {
    const fetchHoldings = async () => {
      try {
        const data = await getHoldings()
        setHoldings(data)
      } catch (err) {
        setError(err.message)
      } finally {
        setLoading(false)
      }
    }
    fetchHoldings()
  }, [])

  return (
    <div>
      <h2>Holdings List</h2>
      {loading && <p>Loading...</p>}
      {error && <p>Error: {error}</p>}
      {!loading && !error && (
        <table>
          <thead>
            <tr>
              <th>Symbol</th>
              <th>Quantity</th>
              <th>Average Buy Price</th>
              <th>Current Price</th>
              <th>Total Value</th>
              <th>Gain/Loss</th>
              <th>Gain/Loss Percent</th>
              <th>Action</th>
            </tr>
          </thead>
          <tbody>
            {holdings.map((holding) => (
              <tr key={holding.symbol}>
                <td>{holding.symbol}</td>
                <td>{holding.quantity}</td>
                <td>${holding.averageBuyPrice}</td>
                <td>${holding.currentPrice}</td>
                <td>${holding.currentValue}</td>
                <td>${holding.gainLoss}</td>
                <td>{holding.gainLossPercent.toFixed(2)}%</td>
                <td>
                  <Link to={`/portfolio/holdings/${holding.symbol}`}>View Details</Link>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  )
}

export default HoldingsList
