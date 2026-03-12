import { getHoldings } from '../../../services/portfolioService'
import { Link } from 'react-router-dom'
import useFetch from '../../../hooks/useFetch'

function HoldingsList() {
  const { data: holdings, loading, error } = useFetch(getHoldings)

  return (
    <div>
      <h2>Liste des actifs</h2>
      {loading && <p>Chargement...</p>}
      {error && <p>Erreur: {error}</p>}
      {!loading && !error && (
        <table>
          <thead>
            <tr>
              <th>Crypto</th>
              <th>Quantité</th>
              <th>Prix d'achat moyen</th>
              <th>Prix actuel</th>
              <th>Valeur totale</th>
              <th>Gain/Perte</th>
              <th>Pourcentage de gain/perte</th>
              <th>Action</th>
            </tr>
          </thead>
          <tbody>
            {(holdings ?? []).map((holding) => (
              <tr key={holding.symbol}>
                <td>{holding.symbol}</td>
                <td>{holding.quantity}</td>
                <td>${holding.averageBuyPrice}</td>
                <td>${holding.currentPrice}</td>
                <td>${holding.currentValue}</td>
                <td>${holding.gainLoss}</td>
                <td>{holding.gainLossPercent.toFixed(2)}%</td>
                <td>
                  <Link to={`/portfolio/holdings/${holding.symbol}`}>Voir les détails</Link>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  )
}

export default HoldingsList
