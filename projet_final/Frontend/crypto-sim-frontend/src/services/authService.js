import { API_BASE_URL, safeFetch } from './api'

export const login = (username, password) =>
  safeFetch('/auth/login', {
    method: 'POST',
    body: JSON.stringify({ username, password }),
  })

export const register = async (username, email, password) => {
  const response = await fetch(`${API_BASE_URL}/auth/register`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ username, email, password }),
  })

  const data = await response.json().catch(() => ({}))

  if (!response.ok) {
    const firstError =
      Object.values(data?.errors || {}).find(
        (messages) => Array.isArray(messages) && messages.length > 0
      )?.[0] || data.message || "Une erreur est survenue lors de l'inscription."
    throw new Error(firstError)
  }

  return data
}

export const getProfile = async () => {
  if (!localStorage.getItem('token')) return null
  return safeFetch('/auth/me')
}
