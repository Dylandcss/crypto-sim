import { useEffect, useState } from 'react'
import { useAuth } from '../../../context/AuthContext'
import { getHoldingDetails } from '../../../services/portfolioService'
import { makeOrder } from '../../../services/orderService'
import { formatBalance, formatPrice } from '../../../utils/formatters'
import DisplayMessage from '../../common/DisplayMessage/DisplayMessage'
import styles from './TradeForm.module.css'

const TradeForm = ({ symbol, currentPrice }) => {
  const { user, refreshUser } = useAuth()
  const [orderMode, setOrderMode] = useState('market') // 'market' | 'limit'
  const [quantity, setQuantity] = useState('')
  const [limitPrice, setLimitPrice] = useState('')
  const [holding, setHolding] = useState(null)
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [message, setMessage] = useState(null)

  useEffect(() => {
    fetchHolding()
  }, [symbol])

  // Refresh du solde pour les ordres limites
  useEffect(() => {
    const interval = setInterval(() => {
      refreshUser()
      fetchHolding()
    }, 5000)
    return () => clearInterval(interval)
  }, [symbol])

  const fetchHolding = async () => {
    try {
      const data = await getHoldingDetails(symbol)
      setHolding(data)
    } catch (err) {
      setHolding({ quantity: 0 })
    }
  }

  const handleTrade = async (type) => {
    if (!quantity || isNaN(quantity) || quantity <= 0) {
      setMessage({ type: 'error', text: 'Veuillez saisir une quantité valide.' })
      return
    }

    const lp = orderMode === 'limit' ? parseFloat(limitPrice) : null

    if (orderMode === 'limit') {
      if (!limitPrice || isNaN(limitPrice) || lp <= 0) {
        setMessage({ type: 'error', text: 'Veuillez saisir un prix limite valide.' })
        return
      }
    } else {
      const totalCost = quantity * currentPrice
      if (type === 'Buy' && totalCost > user.balance) {
        setMessage({ type: 'error', text: 'Solde insuffisant.' })
        return
      }
      if (type === 'Sell' && (!holding || quantity > holding.quantity)) {
        setMessage({ type: 'error', text: 'Quantité insuffisante dans votre portefeuille.' })
        return
      }
    }

    setIsSubmitting(true)
    setMessage(null)

    try {
      await makeOrder(symbol, parseFloat(quantity), type, lp)

      if (orderMode === 'limit') {
        setMessage({
          type: 'success',
          text: `Ordre limite enregistré. Il s'exécutera quand le prix atteindra ${formatPrice(lp)}.`,
        })
      } else {
        setMessage({
          type: 'success',
          text: `Ordre de ${type === 'Buy' ? 'achat' : 'vente'} réussi !`,
        })
        await Promise.all([refreshUser(), fetchHolding()])
      }

      setQuantity('')
      setLimitPrice('')
    } catch (err) {
      setMessage({ type: 'error', text: err.message })
    } finally {
      setIsSubmitting(false)
    }
  }

  const effectivePrice = orderMode === 'limit' && limitPrice ? parseFloat(limitPrice) : currentPrice
  const total = quantity ? parseFloat(quantity) * effectivePrice : 0

  return (
    <div className={styles.tradeForm}>
      {/* Toggle marché / limite */}
      <div className={styles.orderTypeToggle}>
        <button
          className={`${styles.toggleButton} ${orderMode === 'market' ? styles.toggleActive : ''}`}
          onClick={() => setOrderMode('market')}
        >
          Au marché
        </button>
        <button
          className={`${styles.toggleButton} ${orderMode === 'limit' ? styles.toggleActive : ''}`}
          onClick={() => setOrderMode('limit')}
        >
          Ordre limite
        </button>
      </div>

      <div className={styles.balanceInfo}>
        <div className={styles.infoLine}>
          <span className={styles.label}>Solde:</span>
          <span className={styles.value}>{formatBalance(user?.balance)}</span>
        </div>
        <div className={styles.infoLine}>
          <span className={styles.label}>Détenu:</span>
          <span className={styles.value}>
            {holding?.quantity ?? 0} {symbol}
          </span>
        </div>
      </div>

      <div className={styles.inputGroup}>
        <label htmlFor="quantity">Quantité</label>
        <div className={styles.inputWrapper}>
          <input
            id="quantity"
            type="number"
            step="any"
            placeholder="0.00"
            value={quantity}
            onChange={(e) => setQuantity(e.target.value)}
            disabled={isSubmitting}
          />
          <span className={styles.symbolSuffix}>{symbol}</span>
        </div>
      </div>

      {orderMode === 'limit' && (
        <div className={styles.inputGroup}>
          <label htmlFor="limitPrice">Prix cible</label>
          <div className={styles.inputWrapper}>
            <input
              id="limitPrice"
              type="number"
              step="any"
              placeholder="0.00"
              value={limitPrice}
              onChange={(e) => setLimitPrice(e.target.value)}
              disabled={isSubmitting}
            />
            <span className={styles.symbolSuffix}>USD</span>
          </div>
        </div>
      )}

      <div className={styles.summary}>
        <div className={styles.summaryLine}>
          <span>Prix {orderMode === 'limit' ? 'cible' : 'unitaire'}:</span>
          <span>{formatPrice(effectivePrice)}</span>
        </div>
        <div className={styles.summaryLine}>
          <span>Total estimé:</span>
          <strong className={styles.totalValue}>{formatPrice(total)}</strong>
        </div>
      </div>

      {message && (
        <div className={styles.messageBox}>
          <DisplayMessage type={message.type} message={message.text} />
        </div>
      )}

      <div className={styles.actions}>
        <button
          className={`${styles.tradeButton} ${styles.sellButton}`}
          onClick={() => handleTrade('Sell')}
          disabled={
            isSubmitting ||
            !quantity ||
            (orderMode === 'market' && (!holding || (holding.quantity ?? 0) <= 0))
          }
        >
          {isSubmitting ? '...' : 'Vendre'}
        </button>
        <button
          className={`${styles.tradeButton} ${styles.buyButton}`}
          onClick={() => handleTrade('Buy')}
          disabled={isSubmitting || !quantity}
        >
          {isSubmitting ? '...' : 'Acheter'}
        </button>
      </div>
    </div>
  )
}

export default TradeForm
