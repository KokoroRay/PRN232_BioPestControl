import React from 'react';
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
  Filler
} from 'chart.js';
import { Line } from 'react-chartjs-2';
import type { RevenueStat } from '../../types/statistics';

ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
  Filler
);

interface RevenueChartProps {
  data: RevenueStat[];
}

export const RevenueChart: React.FC<RevenueChartProps> = ({ data }) => {
  const chartData = {
    labels: data.map(item => new Date(item.date).toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit' })),
    datasets: [
      {
        label: 'Revenue',
        data: data.map(item => item.revenue),
        fill: true,
        backgroundColor: 'rgba(163, 230, 53, 0.1)', // primary color with opacity
        borderColor: '#A3E635', // primary color
        tension: 0.4,
        pointRadius: 4,
        pointBackgroundColor: '#fff',
        pointBorderWidth: 2,
        pointHoverRadius: 6,
      },
    ],
  };

  const options = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        display: false,
      },
      tooltip: {
        backgroundColor: '#1e293b',
        titleFont: { size: 12, weight: 'bold' as const },
        bodyFont: { size: 12 },
        padding: 12,
        cornerRadius: 8,
        displayColors: false,
      },
    },
    scales: {
      y: {
        beginAtZero: true,
        grid: {
          display: true,
          color: '#f1f5f9',
        },
        ticks: {
          font: { size: 11 },
          color: '#64748b',
          callback: (value: number | string) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND', maximumFractionDigits: 0 }).format(Number(value)),
        },
      },
      x: {
        grid: {
          display: false,
        },
        ticks: {
          font: { size: 11 },
          color: '#64748b',
        },
      },
    },
  };

  return (
    <div className="h-full w-full">
      <Line data={chartData} options={options} />
    </div>
  );
};
