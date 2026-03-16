import { safeFetch } from './api';

export const makeOrder = (symbol, quantity, type, limitPrice = null) =>
  safeFetch('/orders', {
    method: 'POST',
    body: JSON.stringify({
      cryptoSymbol: symbol,
      type,
      quantity,
      ...(limitPrice !== null && { limitPrice }),
    }),
  });

export const cancelOrder = (orderId) =>
  safeFetch(`/orders/${orderId}`, { method: 'DELETE' });

export const getOrdersHistory = () => safeFetch('/orders');
