import { useState, useEffect } from 'react';
import { getTransactionsHistory } from '../../services/portfolioService';
import { getOrdersHistory, cancelOrder } from '../../services/orderService';
import useFetch from '../../hooks/useFetch';
import Loader from '../../components/common/Loader/Loader';
import DisplayMessage from '../../components/common/DisplayMessage/DisplayMessage';
import { formatPrice } from '../../utils/formatters';
import styles from './HistoryPage.module.css';

const PAGE_SIZE = 10;

const Pagination = ({ page, totalPages, onPrev, onNext }) => {
  if (totalPages <= 1) return null;
  return (
    <div className={styles.pagination}>
      <button className={styles.pageButton} onClick={onPrev} disabled={page === 1}>◀</button>
      <span className={styles.pageInfo}>Page {page} / {totalPages}</span>
      <button className={styles.pageButton} onClick={onNext} disabled={page === totalPages}>▶</button>
    </div>
  );
};

const PaginatedTable = ({ page, totalPages, onPrev, onNext, children }) => (
  <>
    <div className={styles.tableWrapper}>
      {children}
    </div>
    <Pagination page={page} totalPages={totalPages} onPrev={onPrev} onNext={onNext} />
  </>
);

const HistoryPage = () => {
  const [activeTab, setActiveTab] = useState('orders');
  const [ordersRefreshKey, setOrdersRefreshKey] = useState(0);
  const [cancellingId, setCancellingId] = useState(null);
  const [cancelError, setCancelError] = useState('');
  const [ordersPage, setOrdersPage] = useState(1);
  const [txPage, setTxPage] = useState(1);

  const { data: orders, loading: loadingOrders, error: errorOrders } = useFetch(getOrdersHistory, [ordersRefreshKey]);
  const { data: transactions, loading: loadingTransactions, error: errorTransactions } = useFetch(getTransactionsHistory);

  useEffect(() => { setOrdersPage(1); }, [ordersRefreshKey]);

  const getStatusColor = (status) => {
    switch (status) {
      case 'Executed': return styles.statusExecuted;
      case 'Pending': return styles.statusPending;
      case 'Cancelled': return styles.statusCancelled;
      case 'Rejected': return styles.statusRejected;
      default: return '';
    }
  };

  const handleCancel = async (orderId) => {
    setCancellingId(orderId);
    setCancelError('');
    try {
      await cancelOrder(orderId);
      setOrdersRefreshKey(k => k + 1);
    } catch (err) {
      setCancelError(err.message);
    } finally {
      setCancellingId(null);
    }
  };

  const renderOrders = () => {
    if (loadingOrders) return <Loader message="Chargement des ordres..." />;
    if (errorOrders) return <DisplayMessage type="error" message={errorOrders} />;

    const all = (orders ?? []).slice().reverse();

    if (all.length === 0) {
      return <DisplayMessage type="info" message="Aucun ordre passé pour le moment." />;
    }

    const totalPages = Math.max(1, Math.ceil(all.length / PAGE_SIZE));
    const page = Math.min(ordersPage, totalPages);
    const paginated = all.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE);

    return (
      <PaginatedTable
        page={page}
        totalPages={totalPages}
        onPrev={() => setOrdersPage(p => p - 1)}
        onNext={() => setOrdersPage(p => p + 1)}
      >
        <table className={styles.historyTable}>
          <thead>
            <tr>
              <th>Crypto</th>
              <th>Type</th>
              <th>Quantité</th>
              <th>Prix</th>
              <th>Prix limite</th>
              <th>Total</th>
              <th>Statut</th>
              <th>Date</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {paginated.map((order, i) => (
              <tr key={order.orderId ?? `order-${i}`}>
                <td className={styles.symbol}>{order.cryptoSymbol}</td>
                <td className={order.type === 'Buy' ? styles.buyType : styles.sellType}>
                  {order.type === 'Buy' ? 'Achat' : 'Vente'}
                </td>
                <td>{order.quantity}</td>
                <td>{formatPrice(order.price)}</td>
                <td>{order.limitPrice ? formatPrice(order.limitPrice) : '-'}</td>
                <td>{formatPrice(order.total)}</td>
                <td>
                  <span className={`${styles.statusBadge} ${getStatusColor(order.status)}`}>
                    {order.status}
                  </span>
                </td>
                <td>{new Date(order.executedAt).toLocaleString()}</td>
                <td>
                  {order.status === 'Pending' && (
                    <button
                      className={styles.cancelButton}
                      onClick={() => handleCancel(order.orderId)}
                      disabled={cancellingId === order.orderId}
                    >
                      {cancellingId === order.orderId ? '...' : 'Annuler'}
                    </button>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </PaginatedTable>
    );
  };

  const renderTransactions = () => {
    if (loadingTransactions) return <Loader message="Chargement des transactions..." />;
    if (errorTransactions) return <DisplayMessage type="error" message={errorTransactions} />;

    const all = (transactions ?? []).slice().reverse();

    if (all.length === 0) {
      return <DisplayMessage type="info" message="Aucune transaction exécutée pour le moment." />;
    }

    const totalPages = Math.max(1, Math.ceil(all.length / PAGE_SIZE));
    const page = Math.min(txPage, totalPages);
    const paginated = all.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE);

    return (
      <PaginatedTable
        page={page}
        totalPages={totalPages}
        onPrev={() => setTxPage(p => p - 1)}
        onNext={() => setTxPage(p => p + 1)}
      >
        <table className={styles.historyTable}>
          <thead>
            <tr>
              <th>Crypto</th>
              <th>Type</th>
              <th>Quantité</th>
              <th>Prix Exécution</th>
              <th>Total</th>
              <th>Date d'exécution</th>
            </tr>
          </thead>
          <tbody>
            {paginated.map((tx, i) => (
              <tr key={tx.id ?? `tx-${i}`}>
                <td className={styles.symbol}>{tx.cryptoSymbol}</td>
                <td className={tx.type === 'Buy' ? styles.buyType : styles.sellType}>
                  {tx.type === 'Buy' ? 'Achat' : 'Vente'}
                </td>
                <td>{tx.quantity}</td>
                <td>{formatPrice(tx.priceAtTime)}</td>
                <td>{formatPrice(tx.total)}</td>
                <td>{new Date(tx.executedAt).toLocaleString()}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </PaginatedTable>
    );
  };

  return (
    <div className={styles.historyPage}>
      <div className={styles.historyHeader}>
        <div className={styles.tabGroup}>
          <button
            className={`${styles.tabButton} ${activeTab === 'orders' ? styles.activeTab : ''}`}
            onClick={() => { setActiveTab('orders'); setOrdersPage(1); }}
          >
            Mes Ordres
          </button>
          <button
            className={`${styles.tabButton} ${activeTab === 'transactions' ? styles.activeTab : ''}`}
            onClick={() => { setActiveTab('transactions'); setTxPage(1); }}
          >
            Transactions Exécutées
          </button>
        </div>
      </div>

      {cancelError && (
        <DisplayMessage type="error" message={cancelError} onClose={() => setCancelError('')} />
      )}

      <div className={styles.tabContent}>
        {activeTab === 'orders' ? renderOrders() : renderTransactions()}
      </div>
    </div>
  );
};

export default HistoryPage;
