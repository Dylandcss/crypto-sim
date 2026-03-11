const BASE_URL = 'http://localhost:5000'
const token = localStorage.getItem('token')

export const getPortfolioSummary = async () => {
  const response = await fetch(`${BASE_URL}/api/portfolio`, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
  })

  if (!response.ok) {
    const res = await response.json()
    throw new Error(res.message)
  }

  return response.json()
}

export const getHoldings = async () => {
  const response = await fetch(`${BASE_URL}/api/portfolio/holdings`, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
  })

  if (!response.ok) {
    const res = await response.json()
    throw new Error(res.message)
  }

  return response.json()
}

export const getHoldingDetails = async (symbol) => {
  const response = await fetch(`${BASE_URL}/api/portfolio/holdings/${symbol}`, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
  })

  if (!response.ok) {
    const res = await response.json()
    throw new Error(res.message)
  }

  return response.json()
}

export const getTransactionsHistory = async () => {
  const response = await fetch(`${BASE_URL}/api/portfolio/transactions`, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
  })

  if (!response.ok) {
    const res = await response.json()
    throw new Error(res.message)
  }

  return response.json()
}

export const getPerformanceData = async () => {
  const response = await fetch(`${BASE_URL}/api/portfolio/performance`, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
  })

  if (!response.ok) {
    const res = await response.json()
    throw new Error(res.message)
  }

  return response.json()
}
