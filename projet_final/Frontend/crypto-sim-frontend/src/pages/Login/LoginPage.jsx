import React from 'react'
import './LoginPage.Module.css'
import LoginForm from '../../components/LoginForm/LoginForm'
import { Link, useNavigate } from 'react-router-dom'

function LoginPage() {
  const navigate = useNavigate()

  function onLogin() {
    navigate('/market')
  }

  return (
    <div className="login-page">
      <div className="login-card">
        <h1>CryptoSim</h1>
        <LoginForm onLogin={onLogin} />
        <Link to="/register" className="register-link">
          Vous n'avez pas de compte? Inscrivez-vous ici.
        </Link>
      </div>
    </div>
  )
}

export default LoginPage
