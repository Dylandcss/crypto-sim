import React from 'react';
import MarketItem from '../MarketItem/MarketItem';
import styles from './MarketGrid.module.css';

const MarketGrid = ({ cryptos }) => {
  if (!cryptos || cryptos.length === 0) {
    return <div className={styles.emptyState}>Aucune donnée disponible.</div>;
  }

  return (
    <div className={styles.gridContainer}>
      {cryptos.map((crypto) => (
        <MarketItem key={crypto.symbol} crypto={crypto} />
      ))}
    </div>
  );
};

export default MarketGrid;
