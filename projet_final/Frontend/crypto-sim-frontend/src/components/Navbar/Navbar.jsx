import { useLocation, useNavigate } from 'react-router-dom'
import { useAuth } from '../../context/AuthContext'
import styles from './Navbar.module.css'
import moneyBagIcon from '../../assets/images/moneybag_icon.png'

const PAGE_NAMES = {
  '/market': 'Market',
  '/portfolio': 'Portfolio',
}

function Navbar() {
  const { user, logoutUser } = useAuth()
  const navigate = useNavigate()
  const location = useLocation()

  const pageName = PAGE_NAMES[location.pathname] || 'CryptoSim'

  const handleLogout = () => {
    logoutUser()
    navigate('/')
  }

  return (
    <nav className={styles.navbar}>
      <span className={styles['navbar-page-name']}>{pageName}</span>

      <div className={styles['navbar-right']}>
        <span className={styles['navbar-welcome']}>
          Bonjour, <strong>{user?.username}</strong>
        </span>
        <span className={styles['navbar-separator']} />

        <img className={styles['navbar-moneybag-icon']} src={moneyBagIcon} alt="Moneybag" />
        <span className={styles['navbar-balance']}>
          {Number(user?.balance ?? 0).toLocaleString('us-US', {
            style: 'currency',
            currency: 'USD',
          })}
        </span>
        <button className={styles['navbar-logout']} onClick={handleLogout}>
          Déconnexion
        </button>
      </div>
    </nav>
  )
}

export default Navbar
