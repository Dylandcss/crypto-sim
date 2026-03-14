import { getHoldings } from '../../../services/portfolioService';
import { gainLossColor } from '../../../utils/formatters';
import { Link } from 'react-router-dom';
import useFetch from '../../../hooks/useFetch';
import Loader from '../../common/Loader/Loader';
import DisplayMessage from '../../common/DisplayMessage/DisplayMessage';
import styles from './HoldingsList.module.css';

import { getCoinIcon } from '../../../utils/coinIcons';

function HoldingsList() {
  const { data: holdings, loading, error } = useFetch(getHoldings);

  if (loading) return <Loader message="Chargement de la liste des actifs..." />;
  if (error) return <DisplayMessage type="error" message={error} />;

  return (
    <div className={styles['holdings-list']}>
      <h2>Liste des actifs</h2>
      <table className={styles['holdings-table']}>
        <thead className={styles['holdings-table-header']}>
          <tr>
            <th colSpan={2}>Crypto</th>
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
          {(holdings ?? []).map((holding) => {
            const coinIcon = getCoinIcon(holding.symbol);
            return (
              <tr key={holding.symbol} className={styles['holdings-table-row']}>
                <td style={{ width: '40px', paddingRight: '0' }}>
                  {coinIcon && (
                    <img 
                      src={coinIcon} 
                      alt={holding.symbol} 
                      style={{ 
                        width: '32px', 
                        height: '32px', 
                        verticalAlign: 'middle',
                        filter: 'drop-shadow(0 2px 4px rgba(0,0,0,0.3))'
                      }} 
                    />
                  )}
                </td>
                <td style={{ fontWeight: 'bold' }}>{holding.symbol}</td>
                <td>{holding.quantity}</td>
                <td>${holding.averageBuyPrice}</td>
                <td>${holding.currentPrice}</td>
                <td>${holding.currentValue}</td>
                <td style={{ color: gainLossColor(holding.gainLoss) }}>${holding.gainLoss}</td>
                <td style={{ color: gainLossColor(holding.gainLoss) }}>
                  {holding.gainLossPercent.toFixed(2)}%
                </td>
                <td>
                  <Link to={`/portfolio/holdings/${holding.symbol}`} className={styles.btnDetails}>Voir les détails</Link>
                </td>
              </tr>
            );
          })}
        </tbody>
      </table>
    </div>
  );
}

export default HoldingsList;
