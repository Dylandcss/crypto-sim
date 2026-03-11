// src/services/authService.js
export const login = async (email, password) => {
  // Simulation d'une réponse serveur
  return new Promise((resolve, reject) => {
    setTimeout(() => {
      if (email === "test@test.com" && password === "123456") {
        resolve({ token: "FAKE_JWT_TOKEN" });
      } else {
        reject(new Error("Login failed"));
      }
    }, 500); // Simule délai réseau
  });
};

//fetch et url du back