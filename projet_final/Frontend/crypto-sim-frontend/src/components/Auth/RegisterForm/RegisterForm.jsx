import { useState } from 'react'
import styles from './RegisterForm.module.css'
import { register } from '../../../services/authService'
import { useNavigate } from 'react-router-dom'
import DisplayMessage from '../../common/DisplayMessage/DisplayMessage'
import Loader from '../../common/Loader/Loader'

function RegisterForm() {
  const [username, setUsername] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)
  const navigate = useNavigate()

  const handleSubmit = async (e) => {
    e.preventDefault()
    setError('')
    setLoading(true)
    try {
      await register(username, email, password)
      navigate('/login', {
        state: { message: 'Inscription réussie, veuillez vous connecter.' },
      })
    } catch (error) {
      setError(error.message)
    } finally {
      setLoading(false)
    }
  }

  return (
    <>
      {error && <DisplayMessage type="error" message={error} onClose={() => setError('')} />}
      <form className={styles['register-form']} onSubmit={handleSubmit}>
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
        {loading ? (
          <Loader />
        ) : (
          <button type="submit">S'inscrire</button>
        )}
      </form>
    </>
  )
}

export default RegisterForm
