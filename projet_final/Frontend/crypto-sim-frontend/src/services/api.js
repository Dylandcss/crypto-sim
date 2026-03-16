export const BASE_URL = import.meta.env.VITE_BASE_URL || 'http://localhost:5000'
export const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || `${BASE_URL}/api`
export const SIGNALR_HUB_URL = import.meta.env.VITE_SIGNALR_HUB_URL || 'http://localhost:5002/marketHub'


export const safeFetch = async (endpoint, options = {}) => {
  const token = localStorage.getItem('token');
  const headers = {
    'Content-Type': 'application/json',
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
    ...options.headers,
  };

  const response = await fetch(`${API_BASE_URL}${endpoint}`, { ...options, headers });

  if (response.status === 401) {
    window.dispatchEvent(new Event('unauthorized'));
    throw new Error('Session expirée');
  }

  const data = await response.json().catch(() => ({}));

  if (!response.ok) {
    throw new Error(data.message || `Erreur ${response.status} : ${response.statusText}`);
  }

  return data;
};