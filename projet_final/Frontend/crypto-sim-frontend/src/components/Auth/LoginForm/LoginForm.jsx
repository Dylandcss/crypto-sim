import { useState, useEffect } from 'react'
import styles from './LoginForm.module.css'
import { login } from '../../../services/authService'
import { useLocation, useNavigate } from 'react-router-dom'
import { useAuth } from '../../../context/AuthContext'

function LoginForm() {
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const location = useLocation()
  const navigate = useNavigate()
  const message = location.state?.message || ''
  const { loginUser } = useAuth()

  useEffect(() => {
    if (sessionStorage.getItem('sessionExpired')) {
      setError('Session expirée. Veuillez vous reconnecter.')
      sessionStorage.removeItem('sessionExpired')
    }
  }, [])

  const handleSubmit = async (e) => {
    e.preventDefault()
    setError('')
    try {
      const data = await login(username, password)
      loginUser(data)
      navigate('/market')
    } catch (error) {
      setError(error.message)
    }
  }

  return (
    <>
      {error && <div className={styles['login-error']}>{error}</div>}
      {message && <div className={styles['register-message']}>{message}</div>}
      <form className={styles['login-form']} onSubmit={handleSubmit}>
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
