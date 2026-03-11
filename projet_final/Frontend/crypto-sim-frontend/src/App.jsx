import './App.css'
import LoginPage from "./pages/Login/LoginPage.jsx";
import { Routes, Route } from 'react-router-dom';
import MarketPage from "./pages/Market/MarketPage.jsx";

function App() {
  return (
    <Routes>
        <Route path="/" element={<LoginPage/>} />
        <Route path="/market" element={<MarketPage/>} />
    </Routes>
  )
}

export default App
