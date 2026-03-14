import { BASE_URL, safeFetch } from './api';

const getToken = () => localStorage.getItem('token');

const handleResponse = async (response) => {
  if (response.status === 401) {
    window.dispatchEvent(new Event('unauthorized'));
    throw new Error('Session expirée.');
  }

  if (response.status === 204) return null;

  const res = await response.json();
  if (!response.ok) {
    throw new Error(res.message || 'Une erreur est survenue.');
  }

  return res;
};

const authFetch = (endpoint, options = {}) =>
  safeFetch(`${BASE_URL}${endpoint}`, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${getToken()}`,
      ...options.headers,
    },
  }).then(handleResponse);

/**
 * Passe un ordre d'achat ou de vente
 * @param {string} symbol - Symbole de la crypto (ex: BTC)
 * @param {number} quantity - Quantité à acheter/vendre
 * @param {string} type - 'Buy' ou 'Sell'
 */
export const placeOrder = (symbol, quantity, type, limitPrice = null) => {
  return authFetch('/api/orders', {
    method: 'POST',
    body: JSON.stringify({
      cryptoSymbol: symbol,
      type,
      quantity,
      ...(limitPrice !== null && { limitPrice }),
    })
  });
};

export const cancelOrder = (orderId) =>
  authFetch(`/api/orders/${orderId}`, { method: 'DELETE' });

export const getOrdersHistory = () => authFetch('/api/orders');
