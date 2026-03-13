import * as signalR from '@microsoft/signalr';
import { API_BASE_URL, SIGNALR_HUB_URL } from './api';

/** Récupère le snapshot pour une date précise */
export const getMarketSnapshot = async (date) => {
    try {
        const dateParam = date ? `?date=${new Date(date).toISOString()}` : '';
        const response = await fetch(`${API_BASE_URL}/market/snapshot${dateParam}`);
        if (!response.ok) {
            throw new Error(`Erreur snapshot : ${response.statusText}`);
        }
        return await response.json();
    } catch (error) {
        console.error('getMarketSnapshot error:', error);
        throw error;
    }
};

/** Streaming des prix via SignalR */
export const createSignalRConnection = (onPriceUpdate, onConnectionClosed) => {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl(SIGNALR_HUB_URL)
        .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.on('ReceivePrices', (updates) => {
        if (onPriceUpdate) onPriceUpdate(updates);
    });

    connection.onclose((error) => {
        console.warn('SignalR coupé :', error);
        if (onConnectionClosed) onConnectionClosed(error);
    });

    return connection;
};

/** Liste des cryptos disponibles */
export const getCryptos = async () => {
    try {
        const response = await fetch(`${API_BASE_URL}/market/cryptos`);
        if (!response.ok) throw new Error(`Erreur cryptos : ${response.statusText}`);
        return await response.json();
    } catch (error) {
        console.error('getCryptos error:', error);
        throw error;
    }
};

/** Détails d'une crypto (par symbole) */
export const getCryptoBySymbol = async (symbol) => {
    try {
        const response = await fetch(`${API_BASE_URL}/market/cryptos/${symbol}`);
        if (!response.ok) throw new Error(`Erreur détail ${symbol}`);
        return await response.json();
    } catch (error) {
        console.error(`getCryptoBySymbol (${symbol}) error:`, error);
        throw error;
    }
};

/** Historique des prix */
export const getCryptoPriceHistory = async (symbol, limit = 50, skip = 0) => {
    try {
        const response = await fetch(`${API_BASE_URL}/market/history/${symbol}?limit=${limit}&skip=${skip}`);
        if (!response.ok) throw new Error(`Erreur historique ${symbol}`);
        return await response.json();
    } catch (error) {
        console.error(`getCryptoPriceHistory (${symbol}) error:`, error);
        throw error;
    }
};
