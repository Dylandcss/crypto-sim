import { getHoldings } from '../../../services/portfolioService'
import { Link } from 'react-router-dom'
import useFetch from '../../../hooks/useFetch'
import styles from './HoldingsList.module.css'

function HoldingsList() {
  const { data: holdings, loading, error } = useFetch(getHoldings)

  return (
    <div className={styles['holdings-list']}>
      <h2>Liste des actifs</h2>
      {loading && <p className={styles['loading']}>Chargement de la liste des actifs...</p>}
      {error && <p className={styles['error']}>Erreur: {error}</p>}
      {!loading && !error && (
        <table className={styles['holdings-table']}>
          <thead className={styles['holdings-table-header']}>
            <tr>
              <th>Crypto</th>
              <th>Quantité</th>
              <th>Prix d'achat moyen</th>
              <th>Prix actuel</th>
              <th>Valeur totale</th>
              <th>Gain/Perte</th>
              <th>Pourcentage de gain/perte</th>
              <th>Action</th>
            </tr>
          </thead>
          <tbody className={styles['holdings-table-body']}>
            {(holdings ?? []).map((holding) => (
              <tr key={holding.symbol} className={styles['holdings-table-row']}>
                <td>{holding.symbol}</td>
                <td>{holding.quantity}</td>
                <td>${holding.averageBuyPrice}</td>
                <td>${holding.currentPrice}</td>
                <td>${holding.currentValue}</td>
                <td className={holding.gainLoss >= 0 ? styles['gain'] : styles['loss']}>
                  ${holding.gainLoss}
                </td>
                <td className={holding.gainLoss >= 0 ? styles['gain'] : styles['loss']}>
                  {holding.gainLossPercent.toFixed(2)}%
                </td>
                <td>
                  <Link to={`/portfolio/holdings/${holding.symbol}`}>Voir les détails</Link>
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
