import { getHoldingDetails } from '../../../services/portfolioService';
import { gainLossColor, formatBalance, formatPrice } from '../../../utils/formatters';
import { Link, useParams } from 'react-router-dom';
import useFetch from '../../../hooks/useFetch';
import Loader from '../../../components/common/Loader/Loader';
import DisplayMessage from '../../../components/common/DisplayMessage/DisplayMessage';
import styles from './HoldingDetails.module.css';

function HoldingDetails() {
  const { symbol } = useParams();
  const { data: h, loading, error } = useFetch(
    () => getHoldingDetails(symbol),
    [symbol]
  );

  if (loading) return <Loader message="Chargement..." />;
  if (error) return <DisplayMessage type="error" message={error} />;
  if (!h) return null;

  return (
    <div className={styles['holding-details']}>
      <h2 className={styles['holding-details__title']}>{h.name} <span className={styles['holding-details__symbol']}>{h.symbol}</span></h2>

      <div className={styles['holding-details__rows']}>
        <div className={styles['stat-row']}>
          <span className={styles['stat-label']}>Quantité détenue</span>
          <span className={styles['stat-value']}>{h.quantity}</span>
        </div>
        <div className={styles['stat-row']}>
          <span className={styles['stat-label']}>Prix d'achat moyen</span>
          <span className={styles['stat-value']}>{formatPrice(h.averageBuyPrice)}</span>
        </div>
        <div className={styles['stat-row']}>
          <span className={styles['stat-label']}>Prix actuel</span>
          <span className={styles['stat-value']}>{formatPrice(h.currentPrice)}</span>
        </div>
        <div className={styles['stat-row']}>
          <span className={styles['stat-label']}>Valeur totale</span>
          <span className={styles['stat-value']}>{formatBalance(h.currentValue)}</span>
        </div>
        <div className={styles['stat-row']}>
          <span className={styles['stat-label']}>Gains / Pertes</span>
          <span className={styles['stat-value']} style={{ color: gainLossColor(h.gainLoss) }}>
            {formatBalance(h.gainLoss)}
          </span>
        </div>
        <div className={styles['stat-row']}>
          <span className={styles['stat-label']}>Performance</span>
          <span className={styles['stat-value']} style={{ color: gainLossColor(h.gainLossPercent) }}>
            {h.gainLossPercent.toFixed(2)}%
          </span>
        </div>
      </div>

      <div className={styles['holding-details__actions']}>
        <Link className={styles['holding-details__action-button--buy']} to={`/trade/${h.symbol}`}>
          Acheter
        </Link>
        <Link className={styles['holding-details__action-button--sell']} to={`/trade/${h.symbol}`}>
          Vendre
        </Link>
      </div>
    </div>
  );
}

export default HoldingDetails;
