const globImports = import.meta.glob('../assets/images/coins/*.png', { eager: true });

const coinMap = new Map();

Object.entries(globImports).forEach(([path, module]) => {
  const filename = path.split('/').pop().split('.')[0].toUpperCase();
  const url = module.default || module;
  coinMap.set(filename, url);
});

export const getCoinIconURL = (symbol) => {
  if (!symbol) return null;
  return coinMap.get(symbol.toUpperCase()) || null;
};