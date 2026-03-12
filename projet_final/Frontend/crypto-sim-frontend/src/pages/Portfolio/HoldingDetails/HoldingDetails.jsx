import { getHoldingDetails } from '../../../services/portfolioService'
import { useParams } from 'react-router-dom'
import useFetch from '../../../hooks/useFetch'

function HoldingDetails() {
  const { symbol } = useParams()
  const {
    data: holdingDetails,
    loading,
    error,
  } = useFetch(() => getHoldingDetails(symbol), [symbol])

  return (
    <div>
      <h2>Détails de l'actif</h2>
      {loading && <p>Chargement...</p>}
      {error && <p>Erreur: {error}</p>}
      {!loading && !error && holdingDetails && (
        <div>
          <p>
            <strong>Crypto:</strong> {holdingDetails.symbol}
          </p>
          <p>
            <strong>Nom:</strong> {holdingDetails.name}
          </p>
          <p>
            <strong>Quantité:</strong> {holdingDetails.quantity}
          </p>
          <p>
            <strong>Prix d'achat moyen:</strong> ${holdingDetails.averageBuyPrice}
          </p>
          <p>
            <strong>Prix actuel:</strong> ${holdingDetails.currentPrice}
          </p>
          <p>
            <strong>Valeur totale:</strong> ${holdingDetails.currentValue}
          </p>
          <p>
            <strong>Gain/Perte:</strong> ${holdingDetails.gainLoss}
          </p>
          <p>
            <strong>Pourcentage de gain/perte:</strong> {holdingDetails.gainLossPercent.toFixed(2)}%
          </p>
        </div>
      )}
    </div>
  )
}

export default HoldingDetails
