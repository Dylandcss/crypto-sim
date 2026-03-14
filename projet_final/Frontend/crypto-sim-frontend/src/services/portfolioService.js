import { BASE_URL, safeFetch } from './api';

const getToken = () => localStorage.getItem('token');

const handleResponse = async (response) => {
  if (response.status === 401) {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    sessionStorage.setItem('sessionExpired', 'true');
    // Notifie l'AuthContext pour mettre à jour l'état et rediriger proprement
    window.dispatchEvent(new Event('unauthorized'));
    throw new Error('Session expirée. Veuillez vous reconnecter.');
  }

  if (!response.ok) {
    const res = await response.json();
    throw new Error(res.message);
  }

  return response.json();
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

export const getPortfolioSummary = () => authFetch('/api/portfolio');
export const getHoldings = () => authFetch('/api/portfolio/holdings');
export const getHoldingDetails = (symbol) => authFetch(`/api/portfolio/holdings/${symbol}`);
export const getTransactionsHistory = () => authFetch('/api/portfolio/transactions');
export const getPerformanceData = () => authFetch('/api/portfolio/performance');
