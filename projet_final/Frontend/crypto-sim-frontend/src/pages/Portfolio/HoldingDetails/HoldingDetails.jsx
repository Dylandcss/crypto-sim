import { getHoldingDetails } from '../../../services/portfolioService'
import { useParams } from 'react-router-dom'
import useFetch from '../../../hooks/useFetch'

function HoldingDetails() {
  const { symbol } = useParams()
  const { data: holdingDetails, loading, error } = useFetch(
    () => getHoldingDetails(symbol),
    [symbol]
  )

  return (
    <div>
      <h2>Holding Details</h2>
      {loading && <p>Loading...</p>}
      {error && <p>Error: {error}</p>}
      {!loading && !error && holdingDetails && (
        <div>
          <p>
            <strong>Symbol:</strong> {holdingDetails.symbol}
          </p>
          <p>
            <strong>Name:</strong> {holdingDetails.name}
          </p>
          <p>
            <strong>Quantity:</strong> {holdingDetails.quantity}
          </p>
          <p>
            <strong>Average Buy Price:</strong> ${holdingDetails.averageBuyPrice}
          </p>
          <p>
            <strong>Current Price:</strong> ${holdingDetails.currentPrice}
          </p>
          <p>
            <strong>Total Value:</strong> ${holdingDetails.currentValue}
          </p>
          <p>
            <strong>Gain/Loss:</strong> ${holdingDetails.gainLoss}
          </p>
          <p>
            <strong>Gain/Loss Percent:</strong> {holdingDetails.gainLossPercent.toFixed(2)}%
          </p>
        </div>
      )}
    </div>
  )
}

export default HoldingDetails
