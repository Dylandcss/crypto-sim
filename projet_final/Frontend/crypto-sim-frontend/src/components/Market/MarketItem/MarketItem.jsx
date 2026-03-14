import { useRef, useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { ArrowUp, ArrowDown } from '@nsmr/pixelart-react';
import { formatPrice } from '../../../utils/formatters';
import styles from './MarketItem.module.css';

const PALETTE = [
  'var(--light-blue)',
  'var(--mid-green)',
  'var(--light-orange)',
  'var(--light-red)',
  'var(--light-green)',
  'var(--light-purple)',
  'var(--light-yellow)',
];

const getColorFromSymbol = (symbol) => {
  let hash = 0;
  for (let i = 0; i < symbol.length; i++) {
    hash += symbol.charCodeAt(i) * (i + 1);
  }
  return PALETTE[hash % PALETTE.length];
};

import { getCoinIcon } from '../../../utils/coinIcons';

const MarketItem = ({ crypto }) => {
  const navigate = useNavigate();
  const prevPriceRef = useRef(crypto.currentPrice);
  const [priceDirection, setPriceDirection] = useState('neutral');
  const [imageError, setImageError] = useState(false);

  const coinImageUrl = getCoinIcon(crypto.symbol);

  useEffect(() => {
    const prevPrice = prevPriceRef.current;
    if (crypto.currentPrice > prevPrice) setPriceDirection('up');
    else if (crypto.currentPrice < prevPrice) setPriceDirection('down');
    prevPriceRef.current = crypto.currentPrice;
  }, [crypto.currentPrice]);

  return (
    <div className={styles.itemContainer} onClick={() => navigate(`/trade/${crypto.symbol}`)}>
      <div className={styles.cardHeader}>
        <div
          className={`${styles.iconContainer} ${(!coinImageUrl || imageError) ? styles.fallback : ''}`}
          style={{ 
            backgroundColor: (!coinImageUrl || imageError) ? getColorFromSymbol(crypto.symbol) : 'transparent',
            borderColor: (!coinImageUrl || imageError) ? '#181B24' : 'transparent'
          }}
        >
          {coinImageUrl && !imageError ? (
            <img 
              src={coinImageUrl} 
              alt={crypto.symbol} 
              className={styles.coinImage}
              onError={() => setImageError(true)}
            />
          ) : (
            crypto.symbol.charAt(0).toUpperCase()
          )}
        </div>
        <div className={styles.infoGroup}>
          <span className={styles.name}>{crypto.name}</span>
          <span className={styles.symbol}>{crypto.symbol}</span>
        </div>
      </div>

      <div className={styles.priceGroup}>
        <span className={`${styles.price} ${styles[priceDirection]}`}>
          {formatPrice(crypto.currentPrice)}
          {priceDirection === 'up' && <span className={styles.arrow}><ArrowUp size={16} /></span>}
          {priceDirection === 'down' && <span className={styles.arrow}><ArrowDown size={16} /></span>}
        </span>
      </div>
    </div>
  );
};

export default MarketItem;
