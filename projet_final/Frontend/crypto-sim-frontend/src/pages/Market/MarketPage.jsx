import { useState, useEffect } from 'react';
import MarketGrid from '../../components/Market/MarketGrid/MarketGrid';
import Loader from '../../components/common/Loader/Loader';
import DisplayMessage from '../../components/common/DisplayMessage/DisplayMessage';
import { getCryptos } from '../../services/marketService';
import useSignalR from '../../hooks/useSignalR';
import { Search } from '@nsmr/pixelart-react';
import styles from './MarketPage.module.css';

export default function MarketPage() {
  const [cryptos, setCryptos] = useState([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    getCryptos()
      .then(setCryptos)
      .catch(() => setError('Impossible de charger le marché.'))
      .finally(() => setIsLoading(false));
  }, []);

  const { error: signalRError } = useSignalR((updatedPrices) => {
    setCryptos((prev) =>
      prev.map((crypto) => {
        const update = updatedPrices.find((u) => u.symbol === crypto.symbol);
        return update ? { ...crypto, currentPrice: update.price } : crypto;
      })
    );
  });

  const filteredCryptos = cryptos.filter(
    (c) =>
      c.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
      c.symbol.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const renderContent = () => {
    if (isLoading) {
      return <Loader message="Chargement des actifs en cours..." />;
    }

    if (error || signalRError) {
      return <DisplayMessage type="error" message={error || signalRError} />;
    }

    return <MarketGrid cryptos={filteredCryptos} />;
  };

  return (
    <div style={{ paddingBlock: '2rem' }}>
      <div className={styles.marketHeader}>
        <div className={styles.searchWrapper}>
          <span className={styles.searchIcon}>
            <Search size={24} />
          </span>
          <input
            type="text"
            placeholder="Rechercher une crypto..."
            className={styles.searchInput}
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
          />
        </div>
      </div>

      {renderContent()}
    </div>
  );
}
