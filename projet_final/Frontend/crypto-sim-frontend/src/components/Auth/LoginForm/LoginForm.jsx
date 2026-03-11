import React, { useState } from 'react'
import './LoginForm.Module.css'
import { login } from '../../../services/authService'
import { useLocation } from 'react-router-dom'

function LoginForm({ onLogin }) {
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const location = useLocation()
  const message = location.state?.message || ''
  const handleSubmit = async (e) => {
    e.preventDefault()
    setError('')
    try {
      const data = await login(username, password)
      localStorage.setItem('token', data.token)
      onLogin()
    } catch (error) {
      setError(error.message)
    }
  }

  return (
    <>
      {error && <div className="login-error">{error}</div>}
      {message && <div className="register-message">{message}</div>}
      <form className="login-form" onSubmit={handleSubmit}>
        <h2>Connexion</h2>
        <input
          type="text"
          placeholder="Nom d'utilisateur"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          required
        />
        <input
          type="password"
          placeholder="Mot de passe"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />
        <button type="submit">Se connecter</button>
      </form>
    </>
  )
}

export default LoginForm
