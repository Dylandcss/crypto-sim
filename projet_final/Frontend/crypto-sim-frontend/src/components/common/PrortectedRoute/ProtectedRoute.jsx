import { Navigate, Outlet } from 'react-router-dom'
import { useAuth } from '../../../context/AuthContext.jsx'
import Navbar from '../../Navbar/Navbar.jsx'
import Sidebar from '../../Sidebar/Sidebar.jsx'
import styles from './ProtectedRoute.module.css'

function ProtectedRoute() {
  const { isAuthenticated } = useAuth()

  if (!isAuthenticated) {
    return <Navigate to="/" replace />
  }

  return (
    <div className={styles['app-layout']}>
      <Sidebar />
      <div className={styles['app-main']}>
        <Navbar />
        <div className={styles['app-content']}>
          <Outlet />
        </div>
      </div>
    </div>
  )
}

export default ProtectedRoute
