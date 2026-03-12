import { getPerformanceData } from '../../../services/portfolioService'
import useFetch from '../../../hooks/useFetch'
import styles from './PerformanceCard.module.css'

function PerformanceCard() {
  const { data: performanceData, loading, error } = useFetch(getPerformanceData)

  return (
    <div className={styles['performance-card']}>
      <h2 className={styles['performance-card__title']}>Performance du portfolio</h2>
      {loading && (
        <p className={styles['performance-card__message']}>
          Chargement des données de performance…
        </p>
      )}
      {error && <p className={styles['performance-card__error']}>Erreur: {error}</p>}
      {!loading && !error && performanceData && (
        <div>
          <p>Total investi : ${performanceData.totalInvested}</p>
          <p>Valeur actuelle : ${performanceData.currentValue}</p>
          <p>
            Total Gains/Pertes :{' '}
            <span
              className={
                performanceData.gainLoss >= 0
                  ? styles['performance-card__gain']
                  : styles['performance-card__loss']
              }
            >
              ${performanceData.gainLoss.toFixed(2)}
            </span>
          </p>
          <p>
            Pourcentage de gain/perte :{' '}
            <span
              className={
                performanceData.gainLossPercent >= 0
                  ? styles['performance-card__gain']
                  : styles['performance-card__loss']
              }
            >
              {performanceData.gainLossPercent.toFixed(2)}%
            </span>
          </p>
        </div>
      )}
    </div>
  )
}

export default PerformanceCard
