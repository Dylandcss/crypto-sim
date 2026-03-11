import React from "react";
import "./LoginPage.Module.css";
import LoginForm from "../../components/LoginForm/LoginForm";
import { useNavigate } from "react-router-dom";

function LoginPage() {
  const navigate = useNavigate();

  function onLogin() {
      navigate("/market");
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