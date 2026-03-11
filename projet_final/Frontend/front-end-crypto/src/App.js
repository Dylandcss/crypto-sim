import React, { useState, useEffect } from "react";
import Navbar from "./components/Navbar/Navbar";
import Portefeuille from "./pages/Portefeuille/Portefeuille";
import LoginPage from "./pages/Login/LoginPage";

function App() {
  const [isLoggedIn, setIsLoggedIn] = useState(false);

  useEffect(() => {
    const token = localStorage.getItem("token");
    setIsLoggedIn(!!token);
  }, []);

  const handleLogin = () => setIsLoggedIn(true);

  return (
    <div className="App">
      {isLoggedIn ? (
        <>
          <Navbar />
          <Portefeuille />
        </>
      ) : (
        <LoginPage onLogin={handleLogin} />
      )}
    </div>
  );
}

export default App;
