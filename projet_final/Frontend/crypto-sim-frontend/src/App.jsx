import './App.css'
import LoginPage from './pages/Login/LoginPage.jsx'
import RegisterPage from './pages/Register/RegisterPage.jsx'
import { Routes, Route } from 'react-router-dom'
import MarketPage from './pages/Market/MarketPage.jsx'
import PortfolioPage from './pages/Portfolio/PortfolioPage.jsx'
import HoldingDetails from './pages/HoldingDetails/HoldingDetails.jsx'

function App() {
  return (
    <Routes>
      <Route path="/" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />
      <Route path="/market" element={<MarketPage />} />
      <Route path="/portfolio" element={<PortfolioPage />} />
      <Route path="/portfolio/holdings/:symbol" element={<HoldingDetails />} />
    </Routes>
  )
}

export default App
