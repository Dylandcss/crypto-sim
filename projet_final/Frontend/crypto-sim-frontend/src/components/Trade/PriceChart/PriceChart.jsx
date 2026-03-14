import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Filler,
  Legend,
} from 'chart.js';
import { Line } from 'react-chartjs-2';
import { formatPrice } from '../../../utils/formatters';
import styles from './PriceChart.module.css';

ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Filler, Legend);

const CHART_OPTIONS = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: { display: false },
    tooltip: {
      mode: 'index',
      intersect: false,
      backgroundColor: '#191434',
      titleFont: { family: 'VT323', size: 16 },
      bodyFont: { family: 'VT323', size: 16 },
      borderColor: '#9594a6',
      borderWidth: 1,
    },
  },
  scales: {
    x: {
      grid: { color: 'rgba(255, 255, 255, 0.05)' },
      ticks: {
        color: 'rgba(255, 255, 255, 0.5)',
        font: { family: 'VT323', size: 14 },
        maxRotation: 0,
        autoSkip: true,
        maxTicksLimit: 8,
      },
    },
    y: {
      grid: { color: 'rgba(255, 255, 255, 0.05)' },
      ticks: {
        color: 'rgba(255, 255, 255, 0.5)',
        font: { family: 'VT323', size: 14 },
      },
    },
  },
};

const PriceChart = ({ symbol, data, currentPrice, priceDirection }) => {
  const chartData = {
    labels: data.map((point) =>
      new Date(point.timestamp).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
    ),
    datasets: [
      {
        fill: true,
        label: 'Prix',
        data: data.map((point) => point.price),
        borderColor: '#3EBB7C',
        backgroundColor: 'rgba(110, 205, 133, 0.1)',
        tension: 0.1,
        pointRadius: 1,
        borderWidth: 2,
      },
    ],
  };

  return (
    <div className={styles.chartContainer}>
      <div className={styles.chartHeader}>
        <div className={styles.title}>{symbol}</div>
        <div className={`${styles.currentPrice} ${styles[priceDirection]}`}>
          {formatPrice(currentPrice)}
        </div>
      </div>
      <div style={{ height: '350px' }}>
        <Line data={chartData} options={CHART_OPTIONS} />
      </div>
    </div>
  );
};

export default PriceChart;
