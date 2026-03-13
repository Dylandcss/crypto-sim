# CryptoSim Frontend

Application de simulation de trading de cryptomonnaies.

## Stack

- **React** + **Vite**
- **React Router** pour la navigation
- **CSS Modules** pour le styling

## Lancer le projet

```bash
npm install
npm run dev
```

## Dockeriser le frontend

Le frontend est servi par **Nginx** sur le port `5173` (hôte) vers `80` (conteneur).

### 1) Build + run avec Docker Compose

```bash
docker compose -f docker-compose.frontend.yml up -d --build
```

### 2) Vérifier

- Frontend : http://localhost:5173

### 3) Connexion au backend

- `/api` -> `http://localhost:5000/api`
- `/marketHub` -> `http://localhost:5000/marketHub`

Les URLs peuvent être surchargées au build via variables Vite :

- `VITE_BASE_URL` (défaut: vide)
- `VITE_API_BASE_URL` (défaut: `/api`)
- `VITE_SIGNALR_HUB_URL` (défaut: `/marketHub`)

Exemple PowerShell avant le `docker compose`:

```powershell
$env:VITE_BASE_URL=""
$env:VITE_API_BASE_URL="/api"
$env:VITE_SIGNALR_HUB_URL="/marketHub"
docker compose -f docker-compose.frontend.yml up -d --build
```
