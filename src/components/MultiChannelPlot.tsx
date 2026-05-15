import React, { useEffect, useRef, useState } from 'react';
import { useAppStore } from '../store/useAppStore';
import { Activity, Trash2, Eye, EyeOff } from 'lucide-react';

export const MultiChannelPlot: React.FC = () => {
  const { 
    channelData, 
    channels, 
    isConnected, 
    addChannelData, 
    clearChannelData,
    toggleChannelVisibility,
    showAllChannels,
    hideAllChannels,
    bufferSize,
    plotWidth,
    setBufferSize,
    setPlotWidth
  } = useAppStore();
  
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const [autoScaleY, setAutoScaleY] = useState(true);
  const [yMin, setYMin] = useState(-0.6);
  const [yMax, setYMax] = useState(0.6);

  useEffect(() => {
    if (!isConnected) return;

    const interval = setInterval(() => {
      const timestamp = Date.now();
      const values = Array(8).fill(0).map((_, i) => {
        const baseValue = Math.random() * 0.1 - 0.05;
        const noise = Math.random() * 0.02;
        return baseValue + noise;
      });
      addChannelData({ timestamp, values });
    }, 50);

    return () => clearInterval(interval);
  }, [isConnected, addChannelData]);

  useEffect(() => {
    const canvas = canvasRef.current;
    if (!canvas) return;

    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    const width = canvas.width;
    const height = canvas.height;
    
    ctx.fillStyle = '#ffffff';
    ctx.fillRect(0, 0, width, height);

    ctx.strokeStyle = '#e0e0e0';
    ctx.lineWidth = 1;
    
    const channelHeight = height / 8;
    const padding = 50;
    const plotAreaWidth = width - padding;

    for (let channelIdx = 0; channelIdx < 8; channelIdx++) {
      const channel = channels[channelIdx];
      if (!channel.visible) continue;

      const yOffset = channelIdx * channelHeight;
      
      ctx.beginPath();
      ctx.strokeStyle = '#f0f0f0';
      for (let i = 0; i <= 6; i++) {
        const y = yOffset + (channelHeight / 6) * i;
        ctx.moveTo(padding, y);
        ctx.lineTo(width, y);
      }
      ctx.stroke();

      ctx.beginPath();
      ctx.strokeStyle = '#d0d0d0';
      for (let i = 0; i <= 10; i++) {
        const x = padding + (plotAreaWidth / 10) * i;
        ctx.moveTo(x, yOffset);
        ctx.lineTo(x, yOffset + channelHeight);
      }
      ctx.stroke();

      if (channelData.length > 1) {
        const visibleData = channelData.slice(-plotWidth);
        
        ctx.beginPath();
        ctx.strokeStyle = channel.color;
        ctx.lineWidth = 2;

        visibleData.forEach((data, idx) => {
          const x = padding + (idx / (visibleData.length - 1)) * plotAreaWidth;
          let value = data.values[channelIdx] * channel.gain + channel.offset;
          
          if (autoScaleY) {
            const allValues = channelData.map(d => d.values[channelIdx] * channel.gain + channel.offset);
            const minVal = Math.min(...allValues);
            const maxVal = Math.max(...allValues);
            const range = maxVal - minVal || 1;
            const y = yOffset + channelHeight - ((value - minVal) / range) * (channelHeight - 10);
            if (idx === 0) {
              ctx.moveTo(x, y);
            } else {
              ctx.lineTo(x, y);
            }
          } else {
            const range = yMax - yMin;
            const y = yOffset + channelHeight - ((value - yMin) / range) * (channelHeight - 10);
            if (idx === 0) {
              ctx.moveTo(x, y);
            } else {
              ctx.lineTo(x, y);
            }
          }
        });
        
        ctx.stroke();

        ctx.fillStyle = channel.color;
        ctx.font = '12px Arial';
        ctx.fillText(channel.name, width - 80, yOffset + 20);
      }

      ctx.fillStyle = '#666';
      ctx.font = '10px Arial';
      if (autoScaleY) {
        const allValues = channelData.map(d => d.values[channelIdx] * channel.gain + channel.offset);
        const minVal = Math.min(...allValues);
        const maxVal = Math.max(...allValues);
        ctx.fillText(maxVal.toFixed(2), 5, yOffset + 15);
        ctx.fillText(minVal.toFixed(2), 5, yOffset + channelHeight - 5);
      } else {
        ctx.fillText(yMax.toFixed(2), 5, yOffset + 15);
        ctx.fillText(yMin.toFixed(2), 5, yOffset + channelHeight - 5);
      }
    }

    ctx.fillStyle = '#666';
    ctx.font = '10px Arial';
    ctx.fillText('0', padding - 10, height - 5);
    ctx.fillText(String(plotWidth), width - 30, height - 5);

  }, [channelData, channels, autoScaleY, yMin, yMax, plotWidth]);

  return (
    <div className="bg-white rounded-xl shadow-md border border-slate-200 p-6">
      <div className="flex items-center justify-between mb-4">
        <div className="flex items-center gap-3">
          <div className="p-2 bg-blue-50 rounded-lg">
            <Activity className="w-6 h-6 text-blue-600" />
          </div>
          <div>
            <h2 className="text-lg font-semibold text-slate-800">多通道数据可视化</h2>
            <p className="text-sm text-slate-500">Serial Plot 风格显示</p>
          </div>
        </div>
        <button
          onClick={clearChannelData}
          className="flex items-center gap-2 px-4 py-2 bg-slate-100 text-slate-700 rounded-lg hover:bg-slate-200 transition-colors"
        >
          <Trash2 className="w-4 h-4" />
          清除数据
        </button>
      </div>

      <div className="flex gap-4 mb-4">
        <div className="flex items-center gap-2">
          <label className="text-sm text-slate-600">Buffer Size:</label>
          <input
            type="number"
            value={bufferSize}
            onChange={(e) => setBufferSize(Number(e.target.value))}
            className="w-24 px-2 py-1 border border-slate-300 rounded text-sm"
          />
        </div>
        <div className="flex items-center gap-2">
          <label className="text-sm text-slate-600">Plot Width:</label>
          <input
            type="number"
            value={plotWidth}
            onChange={(e) => setPlotWidth(Number(e.target.value))}
            className="w-24 px-2 py-1 border border-slate-300 rounded text-sm"
          />
        </div>
        <div className="flex items-center gap-2">
          <label className="text-sm text-slate-600">Auto Scale Y:</label>
          <input
            type="checkbox"
            checked={autoScaleY}
            onChange={(e) => setAutoScaleY(e.target.checked)}
            className="w-4 h-4 rounded"
          />
        </div>
        {!autoScaleY && (
          <>
            <div className="flex items-center gap-2">
              <label className="text-sm text-slate-600">Y Min:</label>
              <input
                type="number"
                step="0.1"
                value={yMin}
                onChange={(e) => setYMin(Number(e.target.value))}
                className="w-20 px-2 py-1 border border-slate-300 rounded text-sm"
              />
            </div>
            <div className="flex items-center gap-2">
              <label className="text-sm text-slate-600">Y Max:</label>
              <input
                type="number"
                step="0.1"
                value={yMax}
                onChange={(e) => setYMax(Number(e.target.value))}
                className="w-20 px-2 py-1 border border-slate-300 rounded text-sm"
              />
            </div>
          </>
        )}
      </div>

      <div className="flex gap-2 mb-4">
        <button
          onClick={showAllChannels}
          className="px-3 py-1 bg-blue-100 text-blue-700 rounded text-sm hover:bg-blue-200"
        >
          Show All
        </button>
        <button
          onClick={hideAllChannels}
          className="px-3 py-1 bg-blue-100 text-blue-700 rounded text-sm hover:bg-blue-200"
        >
          Hide All
        </button>
      </div>

      <div className="bg-slate-50 rounded-lg p-4 mb-4">
        <canvas
          ref={canvasRef}
          width={800}
          height={400}
          className="w-full rounded border border-slate-200"
        />
      </div>

      <div className="bg-slate-50 rounded-lg overflow-x-auto">
        <table className="w-full text-sm">
          <thead>
            <tr className="bg-slate-100">
              <th className="px-3 py-2 text-left font-medium">Channel</th>
              <th className="px-3 py-2 text-center font-medium">Visible</th>
              <th className="px-3 py-2 text-center font-medium">Gain</th>
              <th className="px-3 py-2 text-center font-medium">Offset</th>
            </tr>
          </thead>
          <tbody>
            {channels.map((channel) => (
              <tr key={channel.id} className="border-t border-slate-200 hover:bg-slate-100">
                <td className="px-3 py-2">
                  <span 
                    className="inline-block w-3 h-3 rounded-full mr-2"
                    style={{ backgroundColor: channel.color }}
                  />
                  {channel.name}
                </td>
                <td className="px-3 py-2 text-center">
                  <button
                    onClick={() => toggleChannelVisibility(channel.id)}
                    className="p-1 hover:bg-slate-200 rounded"
                  >
                    {channel.visible ? (
                      <Eye className="w-4 h-4 text-green-600" />
                    ) : (
                      <EyeOff className="w-4 h-4 text-slate-400" />
                    )}
                  </button>
                </td>
                <td className="px-3 py-2 text-center">
                  <input
                    type="number"
                    step="0.1"
                    value={channel.gain}
                    onChange={(e) => {
                      const channelsCopy = [...channels];
                      const idx = channelsCopy.findIndex(c => c.id === channel.id);
                      if (idx !== -1) {
                        channelsCopy[idx].gain = Number(e.target.value);
                      }
                    }}
                    className="w-16 px-2 py-1 border border-slate-300 rounded text-center text-sm"
                  />
                </td>
                <td className="px-3 py-2 text-center">
                  <input
                    type="number"
                    step="0.1"
                    value={channel.offset}
                    onChange={(e) => {
                      const channelsCopy = [...channels];
                      const idx = channelsCopy.findIndex(c => c.id === channel.id);
                      if (idx !== -1) {
                        channelsCopy[idx].offset = Number(e.target.value);
                      }
                    }}
                    className="w-16 px-2 py-1 border border-slate-300 rounded text-center text-sm"
                  />
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};
