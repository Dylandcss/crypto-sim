import { Navigate, Outlet } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import Navbar from './Navbar/Navbar'
import Sidebar from './Sidebar/Sidebar'
import styles from './ProtectedRoute.module.css'

function ProtectedRoute() {
  const { isAuthenticated, isLoading } = useAuth()

  if (isLoading) {
    return null
  }

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
