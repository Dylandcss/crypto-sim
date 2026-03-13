import React, { useState, useEffect, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getCryptoBySymbol, getCryptoPriceHistory, createSignalRConnection } from '../../services/marketService';
import PriceChart from '../../components/Trade/PriceChart/PriceChart';
import Loader from '../../components/common/Loader/Loader';
import DisplayMessage from '../../components/common/DisplayMessage/DisplayMessage';
import { ChevronLeft } from '@nsmr/pixelart-react';
import styles from './TradePage.module.css';

const TradePage = () => {
    const { symbol } = useParams();
    const navigate = useNavigate();
    const [crypto, setCrypto] = useState(null);
    const [history, setHistory] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState(null);
    const [priceDirection, setPriceDirection] = useState('neutral');
    const prevPriceRef = useRef(0);

    useEffect(() => {
        let signalRConnection = null;

        const fetchData = async () => {
            setIsLoading(true);
            setError(null);
            setCrypto(null);
            setHistory([]);

            try {
                const [cryptoData, historyData] = await Promise.all([
                    getCryptoBySymbol(symbol),
                    getCryptoPriceHistory(symbol, 100)
                ]);

                setCrypto(cryptoData);
                setHistory(historyData);
                prevPriceRef.current = cryptoData.currentPrice;
                setIsLoading(false);

                // Connexion temps réel pour le prix
                signalRConnection = createSignalRConnection(
                    (updatedPrices) => {
                        const update = updatedPrices.find(u => u.symbol === symbol);
                        if (update) {
                            setCrypto(prev => {
                                if (!prev) return prev;

                                // Direction du prix
                                if (update.price > prevPriceRef.current) setPriceDirection('up');
                                else if (update.price < prevPriceRef.current) setPriceDirection('down');

                                prevPriceRef.current = update.price;

                                // On ajoute au graphique si c'est la bonne crypto
                                setHistory(prevHistory => {
                                    const newPoint = { timestamp: new Date().toISOString(), price: update.price };
                                    const newHistory = [...prevHistory, newPoint];
                                    return newHistory.length > 30 ? newHistory.slice(1) : newHistory;
                                });

                                return { ...prev, currentPrice: update.price };
                            });
                        }
                    },
                    (err) => {
                        if (err) setError('Lien prix perdu...');
                    }
                );

                await signalRConnection.start();
            } catch (err) {
                console.error(err);
                setError('Erreur lors du chargement des données.');
                setIsLoading(false);
            }
        };

        fetchData();

        return () => {
            if (signalRConnection) signalRConnection.stop();
        };
    }, [symbol]);

    if (isLoading) return <Loader message={`Chargement de ${symbol}...`} />;
    if (error) return <DisplayMessage type="error" message={error} />;
    if (!crypto) return <DisplayMessage type="error" message="Crypto introuvable" />;

    return (
        <div className={styles.tradePage}>
            <button className={styles.backButton} onClick={() => navigate('/market')}>
                <ChevronLeft size={20} /> Retour
            </button>

            <div className={styles.mainLayout}>
                <PriceChart
                    symbol={symbol}
                    data={history}
                    currentPrice={crypto.currentPrice}
                    priceDirection={priceDirection}
                />

                {/* Futur TradeForm ici */}
                <div style={{ background: 'var(--widget-bg)', padding: 'var(--spacing-lg)', borderRadius: 'var(--radius-lg)', border: 'var(--border-thick)', boxShadow: 'var(--shadow-retro)' }}>
                    <h3 style={{ marginBottom: '1rem' }}>Passer un ordre</h3>
                    <p style={{ color: 'var(--text-muted)' }}>Bla bla bla...</p>
                </div>
            </div>
        </div>
    );
};

export default TradePage;
