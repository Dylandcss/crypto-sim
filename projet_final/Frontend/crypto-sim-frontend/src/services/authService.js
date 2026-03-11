// src/services/authService.js

const BASE_URL = 'http://localhost:5000'

export const login = async (username, password) => {
  const response = await fetch(`${BASE_URL}/api/auth/login`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ username, password }),
  })

  if (!response.ok) {
    const res = await response.json()
    throw new Error(res.message)
  }

  return response.json()
}

export const register = async (username, email, password) => {
  const response = await fetch(`${BASE_URL}/api/auth/register`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ username, email, password }),
  })

  if (!response.ok) {
    const res = await response.json()
    for (const key in res.errors) {
      if (res.errors[key]?.length > 0) {
        throw new Error(res.errors[key][0])
      }
    }
  }

  return response.json()
}
