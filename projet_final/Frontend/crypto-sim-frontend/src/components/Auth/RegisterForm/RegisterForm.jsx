import React, { useState } from 'react'
import './RegisterForm.Module.css'
import { register } from '../../../services/authService'
import { useNavigate } from 'react-router-dom'

function RegisterForm() {
  const [username, setUsername] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const navigate = useNavigate()

  const handleSubmit = async (e) => {
    e.preventDefault()
    setError('')
    try {
      await register(username, email, password)
      navigate('/', {
        state: { message: 'Inscription réussie, veuillez vous connecter.' },
      })
    } catch (error) {
      setError(error.message)
    }
  }

  return (
    <>
      {error && <div className="register-error">{error}</div>}
      <form className="register-form" onSubmit={handleSubmit}>
        <h2>Inscription</h2>
        <input
          type="text"
          placeholder="Nom d'utilisateur"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          required
        />
        <input
          type="email"
          placeholder="Adresse e-mail"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
        />
        <input
          type="password"
          placeholder="Mot de passe"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />
        <button type="submit">S'inscrire</button>
      </form>
    </>
  )
}

export default RegisterForm
