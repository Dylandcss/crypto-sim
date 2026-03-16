import { useState } from 'react';
import { getHoldings } from '../../../services/portfolioService';
import { gainLossColor, formatPrice } from '../../../utils/formatters';
import { Link } from 'react-router-dom';
import useFetch from '../../../hooks/useFetch';
import Loader from '../../common/Loader/Loader';
import DisplayMessage from '../../common/DisplayMessage/DisplayMessage';
import styles from './HoldingsList.module.css';
import { getCoinIconURL } from '../../../utils/coinIcons';

const formatFull = (value) =>
  new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
    minimumFractionDigits: 2,
    maximumFractionDigits: 8,
  }).format(value);

function HoldingsList() {
  const { data: holdings, loading, error } = useFetch(getHoldings);
  const [precision, setPrecision] = useState(false);

  const fmt = precision ? formatFull : formatPrice;

  if (loading) return <Loader message="Chargement de la liste des actifs..." />;
  if (error) return <DisplayMessage type="error" message={error} />;

  return (
    <div className={styles['holdings-list']}>
      <div className={styles.listHeader}>
        <h2>Liste des actifs</h2>
        <button
          className={`${styles.precisionToggle} ${precision ? styles.precisionActive : ''}`}
          onClick={() => setPrecision(p => !p)}
        >
          Précision {precision ? 'ON' : 'OFF'}
        </button>
      </div>

      {(!holdings || holdings.length === 0) ? (
        <DisplayMessage type="info" message="Vous ne détenez aucun actif pour le moment." />
      ) : (
        <table className={styles['holdings-table']}>
          <thead className={styles['holdings-table-header']}>
            <tr>
              <th colSpan={2}>Crypto</th>
              <th>Quantité</th>
              <th>Prix d'achat</th>
              <th>Prix actuel</th>
              <th>Valeur totale</th>
              <th>Gain/Perte</th>
              <th>Pourcentage de gain/perte</th>
              <th>Action</th>
            </tr>
          </thead>
          <tbody className={styles['holdings-table-body']}>
            {holdings.map((holding) => {
              const coinIcon = getCoinIconURL(holding.symbol);
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
                  <td>{precision ? holding.quantity : Number(holding.quantity).toFixed(4)}</td>
                  <td>{fmt(holding.averageBuyPrice)}</td>
                  <td>{fmt(holding.currentPrice)}</td>
                  <td>{fmt(holding.currentValue)}</td>
                  <td style={{ color: gainLossColor(holding.gainLoss) }}>{fmt(holding.gainLoss)}</td>
                  <td style={{ color: gainLossColor(holding.gainLoss) }}>
                    {precision ? holding.gainLossPercent.toFixed(6) : holding.gainLossPercent.toFixed(2)}%
                  </td>
                  <td>
                    <Link to={`/portfolio/holdings/${holding.symbol}`} className={styles.btnDetails}>Détails</Link>
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      )}
    </div>
  );
}

export default HoldingsList;
