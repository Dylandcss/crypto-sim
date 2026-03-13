import React, { useState, useEffect } from 'react';
import MarketGrid from '../../components/Market/MarketGrid/MarketGrid';
import Loader from '../../components/common/Loader/Loader';
import DisplayMessage from '../../components/common/DisplayMessage/DisplayMessage';
import { getCryptos, createSignalRConnection } from '../../services/marketService';
import { Search } from '@nsmr/pixelart-react';
import styles from './MarketPage.module.css';

export default function MarketPage() {
    const [cryptos, setCryptos] = useState([]);
    const [searchTerm, setSearchTerm] = useState('');
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState(null);
    const [isLive, setIsLive] = useState(false);

    useEffect(() => {
        let signalRConnection = null;

        const initMarket = async () => {
            try {
                // On récupère les cryptos au chargement
                const data = await getCryptos();
                setCryptos(data);
                setIsLoading(false);

                // Setup de la connexion temps réel
                signalRConnection = createSignalRConnection(
                    (updatedPrices) => {
                        // Mise à jour des prix reçus
                        setCryptos(prevCryptos => {
                            const newCryptos = [...prevCryptos];
                            updatedPrices.forEach(update => {
                                const index = newCryptos.findIndex(c => c.symbol === update.symbol);
                                if (index !== -1) {
                                    newCryptos[index] = { ...newCryptos[index], currentPrice: update.price };
                                }
                            });
                            return newCryptos;
                        });
                    },
                    (err) => {
                        if (err) setError('Lien avec le serveur perdu, reconnexion en cours...');
                    }
                );

                await signalRConnection.start();
                setIsLive(true);
                setError(null);
            } catch (err) {
                console.error("Erreur d'initialisation :", err);
                setError('Impossible de charger le marché.');
                setIsLoading(false);
                setIsLive(false);
            }
        };

        initMarket();

        return () => {
            if (signalRConnection) {
                signalRConnection.stop(); // On ferme proprement
                setIsLive(false);
            }
        };
    }, []);

    const filteredCryptos = cryptos.filter(crypto =>
        crypto.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
        crypto.symbol.toLowerCase().includes(searchTerm.toLowerCase())
    );

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

            {error && <DisplayMessage type="error" message={error} />}

            {isLoading ? (
                <Loader message="Chargement des actifs en cours..." />
            ) : (
                <MarketGrid cryptos={filteredCryptos} />
            )}
        </div>
    );
}