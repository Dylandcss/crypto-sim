import { Routes, Route } from 'react-router-dom';
import { CRTProvider } from './context/CRTContext.jsx';
import CRTOverlay from './components/CRTOverlay/CRTOverlay.jsx';
import LoginPage from './pages/Login/LoginPage.jsx';
import RegisterPage from './pages/Register/RegisterPage.jsx';
import MarketPage from './pages/Market/MarketPage.jsx';
import PortfolioPage from './pages/Portfolio/PortfolioPage.jsx';
import TradePage from './pages/Trade/TradePage.jsx';
import HoldingDetails from './pages/Portfolio/HoldingDetails/HoldingDetails.jsx';
import HistoryPage from './pages/History/HistoryPage.jsx';
import Profil from './components/Profil/Profil.jsx';
import ProtectedRoute from './components/common/PrortectedRoute/ProtectedRoute.jsx';
import NotFoundPage from './pages/NotFound/NotFoundPage.jsx';

function App() {
  return (
    <CRTProvider>
      <CRTOverlay />
      <div id="app-root">
        <Routes>
          <Route path="/" element={<LoginPage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />

          <Route element={<ProtectedRoute />}>
            <Route path="/market" element={<MarketPage />} />
            <Route path="/portfolio" element={<PortfolioPage />} />
            <Route path="/trade/:symbol" element={<TradePage />} />
            <Route path="/portfolio/holdings/:symbol" element={<HoldingDetails />} />
            <Route path="/history" element={<HistoryPage />} />
            <Route path="/profil" element={<Profil />} />
          </Route>

          <Route path="*" element={<NotFoundPage />} />
        </Routes>
      </div>
    </CRTProvider>
  );
}

export default App;
