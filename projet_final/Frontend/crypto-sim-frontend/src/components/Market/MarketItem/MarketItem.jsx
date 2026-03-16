import { useRef, useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { ArrowUp, ArrowDown } from '@nsmr/pixelart-react';
import { formatPrice } from '../../../utils/formatters';
import styles from './MarketItem.module.css';
import { getCoinIconURL } from '../../../utils/coinIcons';

const MarketItem = ({ crypto }) => {
  const navigate = useNavigate();
  const prevPriceRef = useRef(crypto.currentPrice);
  const [priceDirection, setPriceDirection] = useState('neutral');

  const coinImageUrl = getCoinIconURL(crypto.symbol);

  useEffect(() => {
    const prevPrice = prevPriceRef.current;
    if (crypto.currentPrice > prevPrice) setPriceDirection('up');
    else if (crypto.currentPrice < prevPrice) setPriceDirection('down');
    prevPriceRef.current = crypto.currentPrice;
  }, [crypto.currentPrice]);

  return (
    <div className={styles.itemContainer} onClick={() => navigate(`/trade/${crypto.symbol}`)}>
      <div className={styles.cardHeader}>
        <div className={styles.iconContainer}>
          {coinImageUrl && (
            <img
              src={coinImageUrl}
              alt={crypto.symbol}
              className={styles.coinImage}
            />
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
