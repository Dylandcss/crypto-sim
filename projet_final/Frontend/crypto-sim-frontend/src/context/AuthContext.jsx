import { createContext, useContext, useState, useEffect } from 'react'

const AuthContext = createContext(null)

export function AuthProvider({ children }) {
  const [token, setToken] = useState(null)
  const [user, setUser] = useState(null)
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    const savedToken = localStorage.getItem('token')
    const savedUser = localStorage.getItem('user')

    if (savedToken) {
      setToken(savedToken)
    }
    if (savedUser) {
      try {
        setUser(JSON.parse(savedUser))
      } catch {
        localStorage.removeItem('user')
      }
    }

    setIsLoading(false)
  }, [])

  // On enregistre les infos après un login OK
  const loginUser = (data) => {
    const userData = {
      username: data.username,
      role: data.role,
      balance: data.balance,
    }

    setToken(data.token)
    setUser(userData)

    localStorage.setItem('token', data.token)
    localStorage.setItem('user', JSON.stringify(userData))
  }


  const logoutUser = () => {
    setToken(null)
    setUser(null)
    localStorage.removeItem('token')
    localStorage.removeItem('user')
  }

  const isAuthenticated = !!token

  return (
    <AuthContext.Provider
      value={{ token, user, isAuthenticated, isLoading, loginUser, logoutUser }}
    >
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth doit être utilisé à l\'intérieur d\'un <AuthProvider>')
  }
  return context
}
