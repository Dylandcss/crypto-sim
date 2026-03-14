import styles from './LoginPage.module.css'
import LoginForm from '../../components/Auth/LoginForm/LoginForm'
import { Link, Navigate } from 'react-router-dom'
import { useAuth } from '../../context/AuthContext'
import logo from '../../assets/images/logo.png'

function LoginPage() {
  const { isAuthenticated } = useAuth()

  if (isAuthenticated) {
    return <Navigate to="/market" replace />
  }

  return (
    <div className={styles['login-page']}>
      <img src={logo} alt="CryptoSim" className={styles['page-logo']} />
      <div className={styles['login-card']}>
        <LoginForm />
        <Link to="/register" className={styles['register-link']}>
          Vous n'avez pas de compte? Inscrivez-vous ici.
        </Link>
      </div>
    </div>
  )
}

export default LoginPage
