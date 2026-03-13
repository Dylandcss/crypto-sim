import React from 'react';
import { useRef, useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { ArrowUp, ArrowDown } from '@nsmr/pixelart-react';
import styles from './MarketItem.module.css';

const MarketItem = ({ crypto }) => {
  const navigate = useNavigate();
  const prevPriceRef = useRef(crypto.currentPrice);
  const [priceDirection, setPriceDirection] = useState('neutral');

  // Couleur stable basée sur le nom de la crypto
  const getColorFromSymbol = (symbol) => {
    const palette = [
      'var(--light-blue)',
      'var(--mid-green)',
      'var(--light-orange)',
      'var(--light-red)',
      'var(--light-green)',
      'var(--light-purple)',
      'var(--light-yellow)'
    ];
    let hash = 0;
    for (let i = 0; i < symbol.length; i++) {
      hash += symbol.charCodeAt(i) * (i + 1);
    }
    return palette[hash % palette.length];
  };

  const iconColor = getColorFromSymbol(crypto.symbol);
  const iconLetter = crypto.symbol.charAt(0).toUpperCase();

  useEffect(() => {
    const prevPrice = prevPriceRef.current;

    // Up ou down ?
    if (crypto.currentPrice > prevPrice) {
      setPriceDirection('up');
    } else if (crypto.currentPrice < prevPrice) {
      setPriceDirection('down');
    }

    prevPriceRef.current = crypto.currentPrice;

    // On pourrait reset ici, mais on garde la couleur pour le dernier mouvement
    const timer = setTimeout(() => {}, 2000);

    return () => clearTimeout(timer);
  }, [crypto.currentPrice]);

  const handleRowClick = () => {
    navigate(`/trade/${crypto.symbol}`);
  };

  // Formatage propre du prix (USD)
  const formattedPrice = new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
    minimumFractionDigits: crypto.currentPrice < 1 ? 4 : 2,
    maximumFractionDigits: crypto.currentPrice < 1 ? 4 : 2,
  }).format(crypto.currentPrice);

  return (
    <div
      className={styles.itemContainer}
      onClick={handleRowClick}
    >
      <div className={styles.cardHeader}>
        <div className={styles.iconContainer} style={{ backgroundColor: iconColor }}>
          {iconLetter}
        </div>
        <div className={styles.infoGroup}>
          <span className={styles.name}>{crypto.name}</span>
          <span className={styles.symbol}>{crypto.symbol}</span>
        </div>
      </div>
      
      <div className={styles.priceGroup}>
        <span className={`${styles.price} ${styles[priceDirection]}`}>
          {formattedPrice}
          {priceDirection === 'up' && <span className={styles.arrow}><ArrowUp size={16} /></span>}
          {priceDirection === 'down' && <span className={styles.arrow}><ArrowDown size={16} /></span>}
        </span>
      </div>
    </div>
  );
};

export default MarketItem;
