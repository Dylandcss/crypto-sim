import * as signalR from '@microsoft/signalr';
import { API_BASE_URL, SIGNALR_HUB_URL, safeFetch } from './api';

/** Récupère le snapshot pour une date précise */
export const getMarketSnapshot = async (date) => {
  const dateParam = date ? `?date=${new Date(date).toISOString()}` : '';
  const response = await safeFetch(`${API_BASE_URL}/market/snapshot${dateParam}`);
  if (!response.ok) throw new Error(`Erreur snapshot : ${response.statusText}`);
  return response.json();
};

/** Crée et configure une connexion SignalR (sans la démarrer) */
export const createSignalRConnection = (onPriceUpdate, onConnectionClosed) => {
  const connection = new signalR.HubConnectionBuilder()
    .withUrl(SIGNALR_HUB_URL)
    .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
    .configureLogging(signalR.LogLevel.Warning)
    .build();

  connection.on('ReceivePrices', (updates) => {
    if (onPriceUpdate) onPriceUpdate(updates);
  });

  connection.onclose((error) => {
    if (onConnectionClosed) onConnectionClosed(error);
  });

  return connection;
};

/** Liste des cryptos disponibles */
export const getCryptos = async () => {
  const response = await safeFetch(`${API_BASE_URL}/market/cryptos`);
  if (!response.ok) throw new Error(`Erreur cryptos : ${response.statusText}`);
  return response.json();
};

/** Détails d'une crypto (par symbole) */
export const getCryptoBySymbol = async (symbol) => {
  const response = await safeFetch(`${API_BASE_URL}/market/cryptos/${symbol}`);
  if (!response.ok) throw new Error(`Erreur détail ${symbol}`);
  return response.json();
};

/** Historique des prix */
export const getCryptoPriceHistory = async (symbol, limit = 50, skip = 0) => {
  const response = await safeFetch(`${API_BASE_URL}/market/history/${symbol}?limit=${limit}&skip=${skip}`);
  if (!response.ok) throw new Error(`Erreur historique ${symbol}`);
  return response.json();
};
