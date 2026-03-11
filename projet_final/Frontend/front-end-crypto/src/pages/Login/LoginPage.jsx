import React from "react";
import "./LoginPage.css";
import LoginForm from "../../components/LoginForm/LoginForm";

function LoginPage() {

  function onLogin() {
    console.log("Utilisateur connecté !");
  }

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