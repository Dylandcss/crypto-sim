import { useState, useEffect, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getCryptoBySymbol, getCryptoPriceHistory } from '../../services/marketService';
import PriceChart from '../../components/Trade/PriceChart/PriceChart';
import Loader from '../../components/common/Loader/Loader';
import DisplayMessage from '../../components/common/DisplayMessage/DisplayMessage';
import useSignalR from '../../hooks/useSignalR';
import { ChevronLeft } from '@nsmr/pixelart-react';
import styles from './TradePage.module.css';
import { getCoinIcon } from '../../utils/coinIcons';
import TradeForm from '../../components/Trade/TradeForm/TradeForm';

const TradePage = () => {
  const { symbol } = useParams();
  const navigate = useNavigate();
  const [crypto, setCrypto] = useState(null);
  const [history, setHistory] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);
  const [priceDirection, setPriceDirection] = useState('neutral');
  const prevPriceRef = useRef(0);
  const coinIcon = getCoinIcon(symbol);

  useEffect(() => {
    setIsLoading(true);
    setError(null);
    setCrypto(null);
    setHistory([]);

    Promise.all([getCryptoBySymbol(symbol), getCryptoPriceHistory(symbol, 100)])
      .then(([cryptoData, historyData]) => {
        setCrypto(cryptoData);
        setHistory(historyData);
        prevPriceRef.current = cryptoData.currentPrice;
      })
      .catch(() => setError('Erreur lors du chargement des données.'))
      .finally(() => setIsLoading(false));
  }, [symbol]);

  const { error: signalRError } = useSignalR((updatedPrices) => {
    const update = updatedPrices.find((u) => u.symbol === symbol);
    if (!update) return;

    if (update.price > prevPriceRef.current) setPriceDirection('up');
    else if (update.price < prevPriceRef.current) setPriceDirection('down');
    prevPriceRef.current = update.price;

    setCrypto((prev) => (prev ? { ...prev, currentPrice: update.price } : prev));
    setHistory((prev) => {
      const next = [...prev, { timestamp: new Date().toISOString(), price: update.price }];
      return next.length > 30 ? next.slice(1) : next;
    });
  });

  if (isLoading) return <Loader message={`Chargement de ${symbol}...`} />;
  if (error || signalRError) return <DisplayMessage type="error" message={error || signalRError} />;
  if (!crypto) return <DisplayMessage type="error" message="Crypto introuvable" />;

  return (
    <div className={styles.tradePage}>
      <div className={styles.header}>
        <button className={styles.backButton} onClick={() => navigate('/market')}>
          <ChevronLeft size={20} /> Retour
        </button>

        <div className={styles.cryptoHeader}>
          {coinIcon && (
            <img src={coinIcon} alt={symbol} className={styles.coinIcon} />
          )}
          <div className={styles.titleGroup}>
            <h1 className={styles.cryptoName}>{crypto.name}</h1>
          </div>
        </div>
      </div>

      <div className={styles.mainLayout}>
        <PriceChart
          symbol={symbol}
          data={history}
          currentPrice={crypto.currentPrice}
          priceDirection={priceDirection}
        />

        <div className={styles.tradeFormWrapper}>
          <h3 className={styles.tradeHeading}>Passer un ordre</h3>
          <TradeForm symbol={symbol} currentPrice={crypto.currentPrice} />
        </div>
      </div>
    </div>
  );
};

export default TradePage;
