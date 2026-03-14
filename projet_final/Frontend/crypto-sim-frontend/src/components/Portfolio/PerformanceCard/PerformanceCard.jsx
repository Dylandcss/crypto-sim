import { getPerformanceData } from '../../../services/portfolioService';
import { gainLossColor } from '../../../utils/formatters';
import useFetch from '../../../hooks/useFetch';
import Loader from '../../common/Loader/Loader';
import DisplayMessage from '../../common/DisplayMessage/DisplayMessage';
import styles from './PerformanceCard.module.css';

function PerformanceCard() {
  const { data: performanceData, loading, error } = useFetch(getPerformanceData);

  if (loading) return <Loader message="Chargement des données de performance…" />;
  if (error) return <DisplayMessage type="error" message={error} />;

  return (
    <div className={styles['performance-card']}>
      <h2 className={styles['performance-card__title']}>Performance du portfolio</h2>
      {performanceData && (
        <div>
          <p>Total investi : ${performanceData.totalInvested}</p>
          <p>Valeur actuelle : ${performanceData.currentValue}</p>
          <p>
            Total Gains/Pertes :{' '}
            <span style={{ color: gainLossColor(performanceData.gainLoss) }}>
              ${performanceData.gainLoss.toFixed(2)}
            </span>
          </p>
          <p>
            Pourcentage de gain/perte :{' '}
            <span style={{ color: gainLossColor(performanceData.gainLossPercent) }}>
              {performanceData.gainLossPercent.toFixed(2)}%
            </span>
          </p>
        </div>
      )}
    </div>
  );
}

export default PerformanceCard;
