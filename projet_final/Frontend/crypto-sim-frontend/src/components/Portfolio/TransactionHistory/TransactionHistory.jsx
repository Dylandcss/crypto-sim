import { getTransactionsHistory } from '../../../services/portfolioService'
import useFetch from '../../../hooks/useFetch'

function TransactionHistory() {
  const { data: transactionHistory, loading, error } = useFetch(getTransactionsHistory)

  return (
    <div>
      <h2>Historique des transactions</h2>
      {loading && <p>Chargement...</p>}
      {error && <p>Erreur: {error}</p>}
      {!loading && !error && (
        <table>
          <thead>
            <tr>
              <th>Crypto</th>
              <th>Quantité</th>
              <th>Type</th>
              <th>Prix au moment de l'exécution</th>
              <th>Total</th>
              <th>Exécuté le</th>
            </tr>
          </thead>
          <tbody>
            {(transactionHistory ?? []).map((tx) => (
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
