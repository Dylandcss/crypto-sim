import * as signalR from '@microsoft/signalr'
import { safeFetch, SIGNALR_HUB_URL } from './api'

export const createSignalRConnection = (onPriceUpdate, onConnectionClosed) => {
  const connection = new signalR.HubConnectionBuilder()
    .withUrl(SIGNALR_HUB_URL)
    .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
    .configureLogging(signalR.LogLevel.Critical)
    .build()

  connection.on('ReceivePrices', (updates) => {
    if (onPriceUpdate) onPriceUpdate(updates)
  })

  connection.onclose((error) => {
    if (onConnectionClosed) onConnectionClosed(error)
  })

  return connection
}

export const getCryptos = () => safeFetch('/market/cryptos')
export const getCryptoBySymbol = (symbol) => safeFetch(`/market/cryptos/${symbol}`)
export const getCryptoPriceHistory = (symbol, limit = 50, skip = 0) =>
  safeFetch(`/market/history/${symbol}?limit=${limit}&skip=${skip}`)
