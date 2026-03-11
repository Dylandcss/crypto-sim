import styles from './LoginPage.module.css'
import LoginForm from '../../components/Auth/LoginForm/LoginForm'
import { Link, Navigate } from 'react-router-dom'
import { useAuth } from '../../context/AuthContext'

function LoginPage() {
  const { isAuthenticated } = useAuth()

  if (isAuthenticated) {
    return <Navigate to="/market" replace />
  }

  return (
    <div className={styles['login-page']}>
      <div className={styles['login-card']}>
        <h1>CryptoSim</h1>
        <LoginForm />
        <Link to="/register" className={styles['register-link']}>
          Vous n'avez pas de compte? Inscrivez-vous ici.
        </Link>
      </div>
    </div>
  )
}

export default LoginPage
