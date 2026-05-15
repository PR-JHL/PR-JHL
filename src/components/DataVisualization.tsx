import React, { useEffect, useRef } from 'react';
import { Line } from 'react-chartjs-2';
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
  Filler,
} from 'chart.js';
import { Activity, Trash2 } from 'lucide-react';
import { useAppStore } from '../store/useAppStore';

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

export const DataVisualization: React.FC = () => {
  const { sensorData, clearSensorData, isConnected, addSensorData } = useAppStore();

  useEffect(() => {
    if (!isConnected) return;

    const interval = setInterval(() => {
      const timestamp = Date.now();
      const value = Math.sin(timestamp / 500) * 2 + Math.random() * 0.5;
      const voltage = 2.5 + value * 0.5;
      addSensorData({ timestamp, value, voltage });
    }, 50);

    return () => clearInterval(interval);
  }, [isConnected, addSensorData]);

  const chartData = {
    labels: sensorData.slice(-100).map((_, i) => i),
    datasets: [
      {
        label: '压电信号',
        data: sensorData.slice(-100).map((d) => d.value),
        borderColor: '#2563eb',
        backgroundColor: 'rgba(37, 99, 235, 0.1)',
        borderWidth: 2,
        fill: true,
        tension: 0.4,
        pointRadius: 0,
        pointHoverRadius: 4,
      },
    ],
  };

  const chartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        display: false,
      },
      tooltip: {
        enabled: true,
      },
    },
    scales: {
      x: {
        display: true,
        grid: {
          color: 'rgba(0, 0, 0, 0.05)',
        },
        ticks: {
          display: false,
        },
      },
      y: {
        display: true,
        grid: {
          color: 'rgba(0, 0, 0, 0.05)',
        },
      },
    },
  };

  const stats = sensorData.length > 0 ? {
    latest: sensorData[sensorData.length - 1].value.toFixed(3),
    max: Math.max(...sensorData.map(d => d.value)).toFixed(3),
    min: Math.min(...sensorData.map(d => d.value)).toFixed(3),
    avg: (sensorData.reduce((sum, d) => sum + d.value, 0) / sensorData.length).toFixed(3),
  } : { latest: '0.000', max: '0.000', min: '0.000', avg: '0.000' };

  return (
    <div className="bg-white rounded-xl shadow-md border border-slate-200 p-6">
      <div className="flex items-center justify-between mb-6">
        <div className="flex items-center gap-3">
          <div className="p-2 bg-blue-50 rounded-lg">
            <Activity className="w-6 h-6 text-blue-600" />
          </div>
          <div>
            <h2 className="text-lg font-semibold text-slate-800">数据可视化</h2>
            <p className="text-sm text-slate-500">实时压电信号监测</p>
          </div>
        </div>
        <button
          onClick={clearSensorData}
          className="flex items-center gap-2 px-4 py-2 bg-slate-100 text-slate-700 rounded-lg hover:bg-slate-200 transition-colors"
        >
          <Trash2 className="w-4 h-4" />
          清除数据
        </button>
      </div>

      <div className="grid grid-cols-4 gap-4 mb-6">
        <div className="p-4 bg-slate-50 rounded-lg">
          <p className="text-sm text-slate-500 mb-1">最新值</p>
          <p className="text-xl font-bold text-blue-600 font-mono">{stats.latest}</p>
        </div>
        <div className="p-4 bg-slate-50 rounded-lg">
          <p className="text-sm text-slate-500 mb-1">最大值</p>
          <p className="text-xl font-bold text-green-600 font-mono">{stats.max}</p>
        </div>
        <div className="p-4 bg-slate-50 rounded-lg">
          <p className="text-sm text-slate-500 mb-1">最小值</p>
          <p className="text-xl font-bold text-red-600 font-mono">{stats.min}</p>
        </div>
        <div className="p-4 bg-slate-50 rounded-lg">
          <p className="text-sm text-slate-500 mb-1">平均值</p>
          <p className="text-xl font-bold text-slate-700 font-mono">{stats.avg}</p>
        </div>
      </div>

      <div className="h-64 rounded-lg overflow-hidden">
        {isConnected ? (
          <Line data={chartData} options={chartOptions} />
        ) : (
          <div className="h-full flex items-center justify-center bg-slate-50 rounded-lg">
            <p className="text-slate-400">请先连接设备以查看数据</p>
          </div>
        )}
      </div>
    </div>
  );
};
