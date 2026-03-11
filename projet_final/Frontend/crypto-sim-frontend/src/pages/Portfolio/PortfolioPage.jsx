import HoldingsList from '../../components/Portfolio/HoldingsList/HoldingsList'
import PortfolioChart from '../../components/Portfolio/PortfolioChart/PortfolioChart'
import PortfolioSummary from '../../components/Portfolio/PortfolioSummary/PortfolioSummary'
import TransactionHistory from '../../components/Portfolio/TransactionHistory/TransactionHistory'
import PerformanceCard from '../../components/Portfolio/PerformanceCard/PerformanceCard'

function PortfolioPage() {
  return (
    <div className="portfolio-page">
      <h1>Portfolio Page</h1>
      <PortfolioSummary />
      <HoldingsList />
      <TransactionHistory />
      <PortfolioChart />
      <PerformanceCard />
    </div>
  )
}

export default PortfolioPage
