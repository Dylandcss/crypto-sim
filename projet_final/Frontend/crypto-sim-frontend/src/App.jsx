import LoginPage from './pages/Login/LoginPage.jsx'
import RegisterPage from './pages/Register/RegisterPage.jsx'
import { Routes, Route } from 'react-router-dom'
import MarketPage from './pages/Market/MarketPage.jsx'
import PortfolioPage from './pages/Portfolio/PortfolioPage.jsx'
import HoldingDetails from './pages/Portfolio/HoldingDetails/HoldingDetails.jsx'
import ProtectedRoute from './components/ProtectedRoute.jsx'
import ProfilPage from './pages/Profil/ProfilPage.jsx'

function App() {
  return (
    <Routes>
      {/* Routes publiques */}
      <Route path="/" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />

      {/* ici on va mettre les routes protégées !!!! */}
      <Route element={<ProtectedRoute />}>
        <Route path="/market" element={<MarketPage />} />
        <Route path="/portfolio" element={<PortfolioPage />} />
        <Route path="/profil" element={<ProfilPage />} />
        <Route path="/portfolio/holdings/:symbol" element={<HoldingDetails />} />
      </Route>
    </Routes>
  )
}

export default App
