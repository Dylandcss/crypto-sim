// On charge toutes les images du dossier coins dynamiquement
const coinImages = import.meta.glob('../assets/images/coins/*.png', { eager: true });

/**
 * Récupère l'URL d'une icône de crypto à partir de son symbole.
 * Supporte le fallback si l'image n'existe pas.
 * @param {string} symbol - Le symbole de la crypto (ex: BTC)
 * @returns {string|null} - L'URL de l'image ou null s'il n'y a pas de correspondance
 */
export const getCoinIcon = (symbol) => {
  if (!symbol) return null;
  
  const cleanSymbol = symbol.toUpperCase();
  const path = `/src/assets/images/coins/${cleanSymbol}.png`;
  
  // Dans l'objet retourné par import.meta.glob, les clés sont les chemins relatifs
  // On doit chercher la clé qui se termine par /SYMBOL.png
  const matchKey = Object.keys(coinImages).find(key => 
    key.toUpperCase().endsWith(`/${cleanSymbol}.PNG`)
  );

  if (matchKey) {
    return coinImages[matchKey].default || coinImages[matchKey];
  }

  return null;
};
