import { NavLink } from 'react-router-dom'
import { Store, Wallet, User } from '@nsmr/pixelart-react'
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
          <Store size={20} />
          <span>Marché</span>
        </NavLink>
        <NavLink
          to="/portfolio"
          className={({ isActive }) => `${styles['sidebar-link']} ${isActive ? styles.active : ''}`}
        >
          <Wallet size={20} />
          <span>Portefeuille</span>
        </NavLink>
        <NavLink
          to="/profil"
          className={({ isActive }) => `${styles['sidebar-link']} ${isActive ? styles.active : ''}`}
        >
          <User size={20} />
          <span>Profil</span>
        </NavLink>
      </nav>
    </aside>
  )
}

export default Sidebar
