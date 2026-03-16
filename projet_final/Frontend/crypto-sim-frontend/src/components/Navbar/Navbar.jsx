import {useLocation, useNavigate} from 'react-router-dom'
import {useAuth} from '../../context/AuthContext'
import {formatBalance} from '../../utils/formatters'
import styles from './Navbar.module.css'
import moneyBagIcon from '../../assets/images/moneybag_icon.png'

const PAGE_NAMES = {
  '/market': 'Marché',
  '/portfolio': 'Portefeuille',
  '/history': 'Historique',
  '/profil': 'Profil',
}

function Navbar() {
  const { user, logoutUser } = useAuth()
  const navigate = useNavigate()
  const location = useLocation()

  const tradeMatch = location.pathname.match(/^\/trade\/(.+)$/)
  const holdingMatch = location.pathname.match(/^\/portfolio\/holdings\/(.+)$/)
  const pageName =
    PAGE_NAMES[location.pathname] ||
    (tradeMatch ? `Trade - ${tradeMatch[1]}` : null) ||
    (holdingMatch ? `Détails - ${holdingMatch[1]}` : null) ||
    'CryptoSim'

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
        <span className={styles['navbar-balance']}>{formatBalance(user?.balance)}</span>
        <button className={styles['navbar-logout']} onClick={handleLogout}>
          Déconnexion
        </button>
      </div>
    </nav>
  )
}

export default Navbar
