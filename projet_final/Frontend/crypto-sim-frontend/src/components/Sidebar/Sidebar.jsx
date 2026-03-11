import { NavLink } from 'react-router-dom'
import styles from './Sidebar.module.css'

function Sidebar() {
  return (
    <aside className={styles.sidebar}>
      <div className={styles['sidebar-header']}>
        <div className={styles['sidebar-logo']}>C</div>
        <span className={styles['sidebar-title']}>CryptoSim</span>
      </div>

      <nav className={styles['sidebar-nav']}>
        <NavLink
          to="/market"
          className={({ isActive }) =>
            `${styles['sidebar-link']} ${isActive ? styles.active : ''}`
          }
        >
          Market
        </NavLink>
        <NavLink
          to="/portfolio"
          className={({ isActive }) =>
            `${styles['sidebar-link']} ${isActive ? styles.active : ''}`
          }
        >
          Portfolio
        </NavLink>
      </nav>
    </aside>
  )
}

export default Sidebar
