import { safeFetch } from './api';

export const getPortfolioSummary = () => safeFetch('/portfolio');

export const getHoldings = () => safeFetch('/portfolio/holdings');

export const getHoldingDetails = (symbol) => safeFetch(`/portfolio/holdings/${symbol}`);

export const getTransactionsHistory = () => safeFetch('/portfolio/transactions');

export const getPerformanceData = () => safeFetch('/portfolio/performance');
