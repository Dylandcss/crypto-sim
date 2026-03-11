import React from "react";
import "./LoginPage.css";
import LoginForm from "./LoginForm";

function LoginPage({ onLogin }) {
  return (
    <div className="login-page">
      <div className="login-card">
        <h1>CryptoSim</h1>
        <LoginForm onLogin={onLogin} />
      </div>
    </div>
  );
}

export default LoginPage;