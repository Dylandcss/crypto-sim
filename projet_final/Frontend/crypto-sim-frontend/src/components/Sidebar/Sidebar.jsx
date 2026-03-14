import { NavLink } from 'react-router-dom'
import styles from './Sidebar.module.css'
import logo from '../../assets/images/logo.png'

function Sidebar() {
  return (
    <aside className={styles.sidebar}>
      <div className={styles['sidebar-header']}>
        <img src={logo} alt="CryptoSim" className={styles['sidebar-logo']} />
      </div>

      <nav className={styles['sidebar-nav']}>
        <NavLink
          to="/market"
          className={({ isActive }) => `${styles['sidebar-link']} ${isActive ? styles.active : ''}`}
        >
          Marché
        </NavLink>
        <NavLink
          to="/portfolio"
          className={({ isActive }) => `${styles['sidebar-link']} ${isActive ? styles.active : ''}`}
        >
          Portefeuille
        </NavLink>
      </nav>
    </aside>
  )
}

export default Sidebar
