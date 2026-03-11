import React, { use, useState } from "react";
import "./LoginForm.module.css";
import { login } from "../../services/authService";

function LoginForm({ onLogin }) {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const data = await login(username, password);
      localStorage.setItem("token", data.token);
      onLogin(); // notifie le parent que l'utilisateur est connecté
    } catch (err) {
      alert("Nom d'utilisateur ou mot de passe incorrect !");
    }
  };

  return (
    <form className="login-form" onSubmit={handleSubmit}>
      <h2>Connexion</h2>
      <input
        type="text"
        placeholder="Nom d'utilisateur"
        value={username}
        onChange={(e) => setUsername(e.target.value)}
        required
      />
      <input
        type="password"
        placeholder="Mot de passe"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        required
      />
      <button type="submit">Se connecter</button>
    </form>
  );
}

export default LoginForm;