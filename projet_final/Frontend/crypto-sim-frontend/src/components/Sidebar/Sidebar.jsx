import { NavLink } from 'react-router-dom'
import { Store, Wallet, User, Clock, DeviceTv } from '@nsmr/pixelart-react'
import { useCRT } from '../../context/CRTContext'
import styles from './Sidebar.module.css'
import logo from '../../assets/images/logo.png'

function Sidebar() {
  const { enabled, toggle } = useCRT()

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
          to="/history"
          className={({ isActive }) => `${styles['sidebar-link']} ${isActive ? styles.active : ''}`}
        >
          <Clock size={20} />
          <span>Historique</span>
        </NavLink>

        <NavLink
          to="/profil"
          className={({ isActive }) => `${styles['sidebar-link']} ${isActive ? styles.active : ''}`}
        >
          <User size={20} />
          <span>Profil</span>
        </NavLink>
      </nav>

      <div className={styles['sidebar-footer']}>
        <button
          className={`${styles['crt-toggle']} ${enabled ? styles['crt-active'] : ''}`}
          onClick={toggle}
          title={enabled ? 'Désactiver effet CRT' : 'Activer effet CRT'}
        >
          <DeviceTv size={20} />
          <span>Effet CRT</span>
        </button>
      </div>
    </aside>
  )
}

export default Sidebar
