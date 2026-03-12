import { getPortfolioSummary } from '../../../services/portfolioService'
import styles from './PortfolioSummary.module.css'
import useFetch from '../../../hooks/useFetch'

function PortfolioSummary() {
  const { data: summary, loading, error } = useFetch(getPortfolioSummary)

  return (
    <div className={styles['portfolio-summary']}>
      <h2>Résumé du portefeuille</h2>
      {loading && <p className={styles['loading']}>Chargement du résumé...</p>}
      {error && <p className={styles['error']}>Erreur: {error}</p>}
      {!loading && !error && summary && (
        <div>
          <p>Argent disponible : ${summary.cashBalance}</p>
          <p>Total investi : ${summary.totalInvested}</p>
          <p>Valeur actuelle: ${summary.totalCurrentValue}</p>
          <p>
            Total Gains/Pertes:{' '}
            <span className={summary.totalGainLoss >= 0 ? styles['gain'] : styles['loss']}>
              ${summary.totalGainLoss.toFixed(2)}
            </span>
          </p>
          <p>
            Pourcentage Gain/Perte:{' '}
            <span className={summary.totalGainLoss >= 0 ? styles['gain'] : styles['loss']}>
              {summary.totalGainLossPercent.toFixed(2)}%
            </span>
          </p>
          <p>Nombre d'actifs: {summary.holdings.length}</p>
        </div>
      )}
    </div>
  )
}

export default PortfolioSummary
