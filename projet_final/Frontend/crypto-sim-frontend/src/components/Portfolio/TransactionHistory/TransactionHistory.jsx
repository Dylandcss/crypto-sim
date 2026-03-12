import { getTransactionsHistory } from '../../../services/portfolioService'
import useFetch from '../../../hooks/useFetch'
import styles from './TransactionHistory.module.css'

function TransactionHistory() {
  const { data: transactionHistory, loading, error } = useFetch(getTransactionsHistory)

  return (
    <div className={styles['transactionHistory']}>
      <h2>Historique des transactions</h2>
      {loading && <p>Chargement...</p>}
      {error && <p>Erreur: {error}</p>}
      {!loading && !error && (
        <table className={styles['transactionTable']}>
          <thead className={styles['transactionTableHeader']}>
            <tr className={styles['transactionTableHeaderRow']}>
              <th>Crypto</th>
              <th>Quantité</th>
              <th>Type</th>
              <th>Prix au moment de l'exécution</th>
              <th>Total</th>
              <th>Exécuté le</th>
            </tr>
          </thead>
          <tbody className={styles['transactionTableBody']}>
            {(transactionHistory ?? []).map((tx) => (
              <tr key={tx.id} className={styles['transactionRow']}>
                <td>{tx.cryptoSymbol}</td>
                <td>{tx.quantity}</td>
                <td className={tx.type === 'Buy' ? styles['Buy'] : styles['Sell']}>
                  {tx.type == 'Buy' ? 'Achat' : 'Vente'}
                </td>
                <td>${tx.priceAtTime}</td>
                <td>${tx.total}</td>
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
