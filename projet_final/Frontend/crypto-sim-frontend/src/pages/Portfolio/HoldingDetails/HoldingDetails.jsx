import { getHoldingDetails } from '../../../services/portfolioService';
import { gainLossColor } from '../../../utils/formatters';
import { Link, useParams } from 'react-router-dom';
import useFetch from '../../../hooks/useFetch';
import Loader from '../../../components/common/Loader/Loader';
import DisplayMessage from '../../../components/common/DisplayMessage/DisplayMessage';
import styles from './HoldingDetails.module.css';

function HoldingDetails() {
  const { symbol } = useParams();
  const { data: holdingDetails, loading, error } = useFetch(
    () => getHoldingDetails(symbol),
    [symbol]
  );

  if (loading) return <Loader message="Chargement..." />;
  if (error) return <DisplayMessage type="error" message={error} />;
  if (!holdingDetails) return null;

  return (
    <div>
      <h2>Détails de l'actif</h2>
      <div className={styles['holding-details']}>
        <p className={styles['holding-details__symbol']}>
          <strong>Symbole :</strong> {holdingDetails.symbol}
        </p>
        <p className={styles['holding-details__name']}>
          <strong>Nom :</strong> {holdingDetails.name}
        </p>
        <p className={styles['holding-details__quantity']}>
          <strong>Quantité dans le portefeuille :</strong> {holdingDetails.quantity}
        </p>
        <p className={styles['holding-details__buy-price']}>
          <strong>Prix d'achat moyen :</strong> ${holdingDetails.averageBuyPrice}
        </p>
        <p className={styles['holding-details__current-price']}>
          <strong>Prix actuel :</strong> ${holdingDetails.currentPrice}
        </p>
        <p className={styles['holding-details__total-value']}>
          <strong>Valeur totale :</strong> ${holdingDetails.currentValue}
        </p>
        <p className={styles['holding-details__gain-loss']}>
          <strong>Gain/Perte :</strong>{' '}
          <span style={{ color: gainLossColor(holdingDetails.gainLoss) }}>
            ${holdingDetails.gainLoss}
          </span>
        </p>
        <p className={styles['holding-details__gain-loss-percent']}>
          <strong>Pourcentage de gain/perte :</strong>{' '}
          <span style={{ color: gainLossColor(holdingDetails.gainLossPercent) }}>
            {holdingDetails.gainLossPercent.toFixed(2)}%
          </span>
        </p>
        <div className={styles['holding-details__actions']}>
          <Link
            className={styles['holding-details__action-button--buy']}
            to={`/trade/${holdingDetails.symbol}`}
          >
            Acheter
          </Link>
          <Link
            className={styles['holding-details__action-button--sell']}
            to={`/trade/${holdingDetails.symbol}`}
          >
            Vendre
          </Link>
        </div>
      </div>
    </div>
  );
}

export default HoldingDetails;
