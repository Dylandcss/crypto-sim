import { getHoldingDetails } from '../../services/portfolioService'
import { useState, useEffect } from 'react'
import { useParams } from 'react-router-dom'
import './HoldingDetails.Module.css'

function HoldingDetails() {
  const { symbol } = useParams()
  const [holdingDetails, setHoldingDetails] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)

  useEffect(() => {
    const fetchHoldingDetails = async () => {
      try {
        const data = await getHoldingDetails(symbol)
        setHoldingDetails(data)
      } catch (err) {
        setError(err.message)
      } finally {
        setLoading(false)
      }
    }
    fetchHoldingDetails()
  }, [])

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
