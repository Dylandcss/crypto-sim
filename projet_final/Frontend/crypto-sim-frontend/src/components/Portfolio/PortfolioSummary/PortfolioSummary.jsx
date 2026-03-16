import { getPortfolioSummary } from '../../../services/portfolioService';
import { gainLossColor, formatBalance } from '../../../utils/formatters';
import useFetch from '../../../hooks/useFetch';
import Loader from '../../common/Loader/Loader';
import DisplayMessage from '../../common/DisplayMessage/DisplayMessage';
import styles from './PortfolioSummary.module.css';

function PortfolioSummary() {
  const { data: summary, loading, error } = useFetch(getPortfolioSummary);

  if (loading) return <Loader message="Chargement du résumé..." />;
  if (error) return <DisplayMessage type="error" message={error} />;

  return (
    <div className={styles['portfolio-summary']}>
      <h2 className={styles['portfolio-summary__title']}>Résumé</h2>
      {summary && (
        <div className={styles['portfolio-summary__rows']}>
          <div className={styles['stat-row']}>
            <span className={styles['stat-label']}>Solde disponible</span>
            <span className={styles['stat-value']}>{formatBalance(summary.cashBalance)}</span>
          </div>
          <div className={styles['stat-row']}>
            <span className={styles['stat-label']}>Total investi</span>
            <span className={styles['stat-value']}>{formatBalance(summary.totalInvested)}</span>
          </div>
          <div className={styles['stat-row']}>
            <span className={styles['stat-label']}>Valeur actuelle</span>
            <span className={styles['stat-value']}>{formatBalance(summary.totalCurrentValue)}</span>
          </div>
          <div className={styles['stat-row']}>
            <span className={styles['stat-label']}>Gains / Pertes</span>
            <span className={styles['stat-value']} style={{ color: gainLossColor(summary.totalGainLoss) }}>
              {formatBalance(summary.totalGainLoss)}
            </span>
          </div>
          <div className={styles['stat-row']}>
            <span className={styles['stat-label']}>Performance</span>
            <span className={styles['stat-value']} style={{ color: gainLossColor(summary.totalGainLoss) }}>
              {summary.totalGainLossPercent.toFixed(2)}%
            </span>
          </div>
          <div className={styles['stat-row']}>
            <span className={styles['stat-label']}>Actifs détenus</span>
            <span className={styles['stat-value']}>{summary.holdings.length}</span>
          </div>
        </div>
      )}
    </div>
  );
}

export default PortfolioSummary;
