// src/services/authService.js

const BASE_URL = "http://localhost:5000";

export const login = async (username, password) => {
  const response = await fetch(`${BASE_URL}/api/auth/login`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify({ username, password })
  });

  if (!response.ok) {
    const res = await response.json();
    throw new Error(res.message);
  }

  return response.json();
};

