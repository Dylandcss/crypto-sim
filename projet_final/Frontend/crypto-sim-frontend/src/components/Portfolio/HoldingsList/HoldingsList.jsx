import { getHoldings } from '../../../services/portfolioService'
import { Link } from 'react-router-dom'
import useFetch from '../../../hooks/useFetch'

function HoldingsList() {
  const { data: holdings, loading, error } = useFetch(getHoldings)

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
            {(holdings ?? []).map((holding) => (
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
