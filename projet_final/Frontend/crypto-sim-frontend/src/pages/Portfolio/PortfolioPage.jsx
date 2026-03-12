import HoldingsList from '../../components/Portfolio/HoldingsList/HoldingsList'
import PortfolioChart from '../../components/Portfolio/PortfolioChart/PortfolioChart'
import PortfolioSummary from '../../components/Portfolio/PortfolioSummary/PortfolioSummary'
import TransactionHistory from '../../components/Portfolio/TransactionHistory/TransactionHistory'
import PerformanceCard from '../../components/Portfolio/PerformanceCard/PerformanceCard'
import styles from './PortfolioPage.module.css'

function PortfolioPage() {
  return (
    <div className={styles['portfolio-page']}>
      <div className={styles['portfolio-page__flex']}>
        <PortfolioSummary />
        <PerformanceCard />
        <PortfolioChart />
      </div>
      <HoldingsList />
      <TransactionHistory />
    </div>
  )
}

export default PortfolioPage
