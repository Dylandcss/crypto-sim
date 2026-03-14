import { BASE_URL } from './api';

const getToken = () => localStorage.getItem('token');

const handleResponse = async (response) => {
  if (response.status === 401) {
    window.dispatchEvent(new Event('unauthorized'));
    throw new Error('Session expirée.');
  }

  const res = await response.json();
  if (!response.ok) {
    throw new Error(res.message || 'Une erreur est survenue.');
  }

  return res;
};

const authFetch = (endpoint, options = {}) =>
  fetch(`${BASE_URL}${endpoint}`, {
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
export const placeOrder = (symbol, quantity, type) => {
  return authFetch('/api/orders', {
    method: 'POST',
    body: JSON.stringify({
      cryptoSymbol: symbol,
      type,
      quantity,
    })
  });
};
