export const BASE_URL = import.meta.env.VITE_BASE_URL || 'http://localhost:5000'
export const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || `${BASE_URL}/api`

// SignalR se connecte directement au MarketService (la Gateway HTTP ne supporte pas les WebSockets)
export const SIGNALR_HUB_URL = import.meta.env.VITE_SIGNALR_HUB_URL || 'http://localhost:5002/marketHub'

// Wrapper global : traduit les erreurs réseau en message lisible
export const safeFetch = (...args) =>
  fetch(...args).catch((err) => {
    if (err instanceof TypeError) {
      throw new Error('Impossible de contacter le serveur. Vérifiez que les services sont démarrés.')
    }
    throw err
  })
