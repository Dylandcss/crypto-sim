import RegisterForm from '../../components/Auth/RegisterForm/RegisterForm'
import styles from './RegisterPage.module.css'
import { Navigate } from 'react-router-dom'
import { useAuth } from '../../context/AuthContext'

function RegisterPage() {
  const { isAuthenticated } = useAuth()

  if (isAuthenticated) {
    return <Navigate to="/market" replace />
  }

  return (
    <div className={styles['register-page']}>
      <div className={styles['register-card']}>
        <h1>CryptoSim</h1>
        <RegisterForm />
      </div>
    </div>
  )
}

export default RegisterPage
