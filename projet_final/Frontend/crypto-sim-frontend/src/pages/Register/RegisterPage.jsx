import RegisterForm from '../../components/Auth/RegisterForm/RegisterForm'
import styles from './RegisterPage.module.css'
import { Link, Navigate } from 'react-router-dom'
import { useAuth } from '../../context/AuthContext'
import logo from '../../assets/images/logo.png'

function RegisterPage() {
  const { isAuthenticated } = useAuth()

  if (isAuthenticated) {
    return <Navigate to="/market" replace />
  }

  return (
    <div className={styles['register-page']}>
      <img src={logo} alt="CryptoSim" className={styles['page-logo']} />
      <div className={styles['register-card']}>
        <RegisterForm />
        <Link to="/login" className={styles['login-link']}>
          Déjà un compte ? Connectez-vous ici.
        </Link>
      </div>
    </div>
  )
}

export default RegisterPage
