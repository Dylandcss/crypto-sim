import { createContext, useContext, useState, useEffect, useCallback, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import { getProfile } from '../services/authService';

const AuthContext = createContext(null);

const getInitialUser = () => {
  try {
    const saved = localStorage.getItem('user');
    return saved ? JSON.parse(saved) : null;
  } catch {
    localStorage.removeItem('user');
    return null;
  }
};

export function AuthProvider({ children }) {
  const [token, setToken] = useState(() => localStorage.getItem('token'));
  const [user, setUser] = useState(getInitialUser);
  const navigate = useNavigate();

  useEffect(() => {
    const handleUnauthorized = () => {
      setToken(null);
      setUser(null);
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      navigate('/', { state: { sessionExpired: 'Session expirée. Veuillez vous reconnecter.' } });
    };
    window.addEventListener('unauthorized', handleUnauthorized);
    return () => window.removeEventListener('unauthorized', handleUnauthorized);
  }, [navigate]);

  const loginUser = useCallback((data) => {
    const userData = { username: data.username, role: data.role, balance: data.balance };
    setToken(data.token);
    setUser(userData);
    localStorage.setItem('token', data.token);
    localStorage.setItem('user', JSON.stringify(userData));
  },[]);

  const logoutUser = useCallback(() => {
    setToken(null);
    setUser(null);
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  },[]);

  const refreshUser = useCallback(async () => {
    try {
      const data = await getProfile();
      if (data) {
        const userData = { username: data.username, role: data.role, balance: data.balance };
        setUser(userData);
        localStorage.setItem('user', JSON.stringify(userData));
      }
    } catch {
      // Rien à faire !
    }
  },[]);

  const value = useMemo(() => ({
    token,
    user,
    isAuthenticated: !!token,
    loginUser,
    logoutUser,
    refreshUser
  }), [token, user, loginUser, logoutUser, refreshUser]);

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth doit être utilisé à l'intérieur d'un <AuthProvider>");
  }
  return context;
}