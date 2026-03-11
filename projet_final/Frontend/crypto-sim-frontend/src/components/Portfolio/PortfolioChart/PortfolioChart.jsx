import { getHoldings } from '../../../services/portfolioService'
import { useMemo } from 'react'
import styles from './PortfolioChart.module.css'
import { Chart as ChartJS, ArcElement, Tooltip, Legend } from 'chart.js'
import { Doughnut } from 'react-chartjs-2'
import useFetch from '../../../hooks/useFetch'

ChartJS.register(ArcElement, Tooltip, Legend)

const COLORS = [
  '#6366f1',
  '#22c55e',
  '#f59e0b',
  '#ef4444',
  '#3b82f6',
  '#ec4899',
  '#14b8a6',
  '#a855f7',
]

const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  cutout: '60%',
  plugins: {
    legend: {
      position: 'right',
      labels: {
        color: '#e2e8f0',
        font: { size: 13, weight: '500' },
        padding: 16,
        usePointStyle: true,
        pointStyleWidth: 12,
      },
    },
    tooltip: {
      backgroundColor: '#1e293b',
      titleColor: '#f8fafc',
      bodyColor: '#cbd5e1',
      borderColor: '#334155',
      borderWidth: 1,
      padding: 12,
      callbacks: {
        label: (ctx) => {
          const total = ctx.dataset.data.reduce((a, b) => a + b, 0)
          const pct = ((ctx.parsed / total) * 100).toFixed(1)
          return ` ${ctx.label}: $${ctx.parsed.toLocaleString()} (${pct}%)`
        },
      },
    },
  },
}

function PortfolioChart() {
  const { data: holdings, loading, error } = useFetch(getHoldings)

  const chartData = useMemo(
    () => ({
      labels: (holdings ?? []).map((h) => h.symbol),
      datasets: [
        {
          data: (holdings ?? []).map((h) => h.currentValue),
          backgroundColor: COLORS.slice(0, (holdings ?? []).length),
          borderColor: '#1e293b',
          borderWidth: 3,
          hoverOffset: 8,
        },
      ],
    }),
    [holdings],
  )

  if (loading) {
    return (
      <div className={styles['portfolio-chart']}>
        <h2 className={styles['portfolio-chart__title']}>Répartition du portfolio</h2>
        <p className={styles['portfolio-chart__message']}>Loading chart…</p>
      </div>
    )
  }

  if (error) {
    return (
      <div className={styles['portfolio-chart']}>
        <h2 className={styles['portfolio-chart__title']}>Répartition du portfolio</h2>
        <p className={styles['portfolio-chart__error']}>Error: {error}</p>
      </div>
    )
  }

  if (!holdings || holdings.length === 0) {
    return (
      <div className={styles['portfolio-chart']}>
        <h2 className={styles['portfolio-chart__title']}>Répartition du portfolio</h2>
        <p className={styles['portfolio-chart__message']}>No holdings to display.</p>
      </div>
    )
  }

  return (
    <div className={styles['portfolio-chart']}>
      <h2 className={styles['portfolio-chart__title']}>Répartition du portfolio</h2>
      <div className={styles['portfolio-chart__container']}>
        <Doughnut data={chartData} options={chartOptions} />
      </div>
    </div>
  )
}

export default PortfolioChart
