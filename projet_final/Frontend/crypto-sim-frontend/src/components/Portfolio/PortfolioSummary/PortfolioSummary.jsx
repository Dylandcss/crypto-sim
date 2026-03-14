import { getPortfolioSummary } from '../../../services/portfolioService';
import { gainLossColor } from '../../../utils/formatters';
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
      <h2>Résumé du portefeuille</h2>
      {summary && (
        <div>
          <p>Argent disponible : ${summary.cashBalance}</p>
          <p>Total investi : ${summary.totalInvested}</p>
          <p>Valeur actuelle : ${summary.totalCurrentValue}</p>
          <p>
            Total Gains/Pertes :{' '}
            <span style={{ color: gainLossColor(summary.totalGainLoss) }}>
              ${summary.totalGainLoss.toFixed(2)}
            </span>
          </p>
          <p>
            Pourcentage Gain/Perte :{' '}
            <span style={{ color: gainLossColor(summary.totalGainLoss) }}>
              {summary.totalGainLossPercent.toFixed(2)}%
            </span>
          </p>
          <p>Nombre d'actifs : {summary.holdings.length}</p>
        </div>
      )}
    </div>
  );
}

export default PortfolioSummary;
