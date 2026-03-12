import { getHoldingDetails } from '../../../services/portfolioService'
import { Link, useParams } from 'react-router-dom'
import useFetch from '../../../hooks/useFetch'
import styles from './HoldingDetails.module.css'

function HoldingDetails() {
  const { symbol } = useParams()
  const {
    data: holdingDetails,
    loading,
    error,
  } = useFetch(() => getHoldingDetails(symbol), [symbol])

  return (
    <div>
      <h2>Détails de l'actif</h2>
      {loading && <p>Chargement...</p>}
      {error && <p className={styles['error']}>Erreur: {error}</p>}
      {!loading && !error && holdingDetails && (
        <div className={styles['holding-details']}>
          <p className={styles['holding-details__symbol']}>
            <strong>Symbole:</strong> {holdingDetails.symbol}
          </p>
          <p className={styles['holding-details__name']}>
            <strong>Nom:</strong> {holdingDetails.name}
          </p>
          <p className={styles['holding-details__quantity']}>
            <strong>Quantité dans le portefeuille:</strong> {holdingDetails.quantity}
          </p>
          <p className={styles['holding-details__buy-price']}>
            <strong>Prix d'achat moyen:</strong> ${holdingDetails.averageBuyPrice}
          </p>
          <p className={styles['holding-details__current-price']}>
            <strong>Prix actuel:</strong> ${holdingDetails.currentPrice}
          </p>
          <p className={styles['holding-details__total-value']}>
            <strong>Valeur totale:</strong> ${holdingDetails.currentValue}
          </p>
          <p className={styles['holding-details__gain-loss']}>
            <strong>Gain/Perte:</strong>{' '}
            <span
              className={
                holdingDetails.gainLoss >= 0
                  ? styles['holding-details__gain']
                  : styles['holding-details__loss']
              }
            >
              ${holdingDetails.gainLoss}
            </span>
          </p>
          <p className={styles['holding-details__gain-loss-percent']}>
            <strong>Pourcentage de gain/perte:</strong>{' '}
            <span
              className={
                holdingDetails.gainLossPercent >= 0
                  ? styles['holding-details__gain']
                  : styles['holding-details__loss']
              }
            >
              {holdingDetails.gainLossPercent.toFixed(2)}%
            </span>
          </p>
          <div className={styles['holding-details__actions']}>
            <Link
              className={styles['holding-details__action-button--buy']}
              to={`/portfolio/buy/market/cryptos/${holdingDetails.symbol}`}
            >
              Acheter
            </Link>
            <Link
              className={styles['holding-details__action-button--sell']}
              to={`/portfolio/market/cryptos/${holdingDetails.symbol}`}
            >
              Vendre
            </Link>
          </div>
        </div>
      )}
    </div>
  )
}

export default HoldingDetails
