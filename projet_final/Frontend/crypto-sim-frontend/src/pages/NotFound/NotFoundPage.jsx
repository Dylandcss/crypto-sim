import { useNavigate } from 'react-router-dom'
import { useAuth } from '../../context/AuthContext'
import styles from './NotFoundPage.module.css'

function NotFoundPage() {
  const navigate = useNavigate()
  const { isAuthenticated } = useAuth()

  const handleBack = () => {
    navigate(isAuthenticated ? '/market' : '/')
  }

  return (
    <div className={styles.container}>
      <p className={styles.code}>404</p>
      <p className={styles.title}>PAGE INTROUVABLE</p>
      <p className={styles.subtitle}>Cette adresse n'existe pas.</p>
      <button className={styles.btn} onClick={handleBack}>
        Retour
      </button>
    </div>
  )
}

export default NotFoundPage
