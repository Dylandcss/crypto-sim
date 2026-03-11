import { BASE_URL } from './api'

const getToken = () => localStorage.getItem('token')

const handleResponse = async (response) => {
  if (response.status === 401) {
    localStorage.removeItem('token')
    localStorage.removeItem('user')
    sessionStorage.setItem('sessionExpired', 'true')
    window.location.href = '/'
    throw new Error('Session expirée. Veuillez vous reconnecter.')
  }

  if (!response.ok) {
    const res = await response.json()
    throw new Error(res.message)
  }

  return response.json()
}

const authFetch = async (endpoint, options = {}) => {
  const response = await fetch(`${BASE_URL}${endpoint}`, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${getToken()}`,
      ...options.headers,
    },
  })
  return handleResponse(response)
}

export const getPortfolioSummary = () => authFetch('/api/portfolio', { method: 'GET' })

export const getHoldings = () => authFetch('/api/portfolio/holdings', { method: 'GET' })

export const getHoldingDetails = (symbol) => authFetch(`/api/portfolio/holdings/${symbol}`, { method: 'GET' })

export const getTransactionsHistory = () => authFetch('/api/portfolio/transactions', { method: 'GET' })

export const getPerformanceData = () => authFetch('/api/portfolio/performance', { method: 'GET' })
