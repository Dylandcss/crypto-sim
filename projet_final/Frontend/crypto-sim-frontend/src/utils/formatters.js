/** Formate un prix crypto en USD (4 décimales si < 1$, sinon 2) */
export const formatPrice = (price) =>
  new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
    minimumFractionDigits: price < 1 ? 4 : 2,
    maximumFractionDigits: price < 1 ? 4 : 2,
  }).format(price);

/** Formate un solde en USD (toujours 2 décimales) */
export const formatBalance = (amount) =>
  new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(amount ?? 0);

/** Retourne la couleur CSS selon qu'une valeur est positive ou négative */
export const gainLossColor = (value) =>
  value >= 0 ? 'var(--light-green)' : 'var(--light-red)';
