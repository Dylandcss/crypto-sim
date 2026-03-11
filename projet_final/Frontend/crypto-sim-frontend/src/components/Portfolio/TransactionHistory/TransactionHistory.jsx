import { getTransactionsHistory } from '../../../services/portfolioService'
import { useState, useEffect } from 'react'
import './TransactionHistory.Module.css'

function TransactionHistory() {
  const [transactionHistory, setTransactionHistory] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)

  useEffect(() => {
    const fetchTransactionHistory = async () => {
      try {
        const data = await getTransactionsHistory()
        setTransactionHistory(data)
      } catch (err) {
        setError(err.message)
      } finally {
        setLoading(false)
      }
    }
    fetchTransactionHistory()
  }, [])

  return (
    <div>
      <h2>Transaction History</h2>
      {loading && <p>Loading...</p>}
      {error && <p>Error: {error}</p>}
      {!loading && !error && (
        <table>
          <thead>
            <tr>
              <th>Symbol</th>
              <th>Quantity</th>
              <th>Type</th>
              <th>Price at Time</th>
              <th>Total</th>
              <th>Executed At</th>
            </tr>
          </thead>
          <tbody>
            {transactionHistory.map((tx) => (
              <tr key={tx.id}>
                <td>{tx.cryptoSymbol}</td>
                <td>{tx.quantity}</td>
                <td>{tx.type}</td>
                <td>${tx.priceAtTime.toFixed(2)}</td>
                <td>${tx.total.toFixed(2)}</td>
                <td>{new Date(tx.executedAt).toLocaleString()}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  )
}

export default TransactionHistory
