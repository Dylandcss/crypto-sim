import { getPerformanceData } from '../../../services/portfolioService';
import { gainLossColor, formatBalance } from '../../../utils/formatters';
import useFetch from '../../../hooks/useFetch';
import Loader from '../../common/Loader/Loader';
import DisplayMessage from '../../common/DisplayMessage/DisplayMessage';
import styles from './PerformanceCard.module.css';

function PerformanceCard() {
  const { data: performanceData, loading, error } = useFetch(getPerformanceData);

  if (loading) return <Loader message="Chargement des performances…" />;
  if (error) return <DisplayMessage type="error" message={error} />;

  return (
    <div className={styles['performance-card']}>
      <h2 className={styles['performance-card__title']}>Performance</h2>
      {performanceData && (
        <div className={styles['performance-card__rows']}>
          <div className={styles['stat-row']}>
            <span className={styles['stat-label']}>Total investi</span>
            <span className={styles['stat-value']}>{formatBalance(performanceData.totalInvested)}</span>
          </div>
          <div className={styles['stat-row']}>
            <span className={styles['stat-label']}>Valeur actuelle</span>
            <span className={styles['stat-value']}>{formatBalance(performanceData.currentValue)}</span>
          </div>
          <div className={styles['stat-row']}>
            <span className={styles['stat-label']}>Gains / Pertes</span>
            <span className={styles['stat-value']} style={{ color: gainLossColor(performanceData.gainLoss) }}>
              {formatBalance(performanceData.gainLoss)}
            </span>
          </div>
          <div className={styles['stat-row']}>
            <span className={styles['stat-label']}>Performance</span>
            <span className={styles['stat-value']} style={{ color: gainLossColor(performanceData.gainLossPercent) }}>
              {performanceData.gainLossPercent.toFixed(2)}%
            </span>
          </div>
        </div>
      )}
    </div>
  );
}

export default PerformanceCard;
