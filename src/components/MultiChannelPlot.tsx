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
    const padding = 40;
    const plotAreaWidth = width - padding;

    for (let channelIdx = 0; channelIdx < 8; channelIdx++) {
      const channel = channels[channelIdx];
      if (!channel.visible) continue;

      const yOffset = channelIdx * channelHeight;
      
      ctx.beginPath();
      ctx.strokeStyle = '#f0f0f0';
      for (let i = 0; i <= 4; i++) {
        const y = yOffset + (channelHeight / 4) * i;
        ctx.moveTo(padding, y);
        ctx.lineTo(width, y);
      }
      ctx.stroke();

      ctx.beginPath();
      ctx.strokeStyle = '#e0e0e0';
      for (let i = 0; i <= 8; i++) {
        const x = padding + (plotAreaWidth / 8) * i;
        ctx.moveTo(x, yOffset + 2);
        ctx.lineTo(x, yOffset + channelHeight - 2);
      }
      ctx.stroke();

      if (channelData.length > 1) {
        const visibleData = channelData.slice(-plotWidth);
        
        ctx.beginPath();
        ctx.strokeStyle = channel.color;
        ctx.lineWidth = 1.5;

        visibleData.forEach((data, idx) => {
          const x = padding + (idx / (visibleData.length - 1)) * plotAreaWidth;
          let value = data.values[channelIdx] * channel.gain + channel.offset;
          
          if (autoScaleY) {
            const allValues = channelData.map(d => d.values[channelIdx] * channel.gain + channel.offset);
            const minVal = Math.min(...allValues);
            const maxVal = Math.max(...allValues);
            const range = maxVal - minVal || 1;
            const y = yOffset + channelHeight - 4 - ((value - minVal) / range) * (channelHeight - 8);
            if (idx === 0) {
              ctx.moveTo(x, y);
            } else {
              ctx.lineTo(x, y);
            }
          } else {
            const range = yMax - yMin;
            const y = yOffset + channelHeight - 4 - ((value - yMin) / range) * (channelHeight - 8);
            if (idx === 0) {
              ctx.moveTo(x, y);
            } else {
              ctx.lineTo(x, y);
            }
          }
        });
        
        ctx.stroke();

        ctx.fillStyle = channel.color;
        ctx.font = '10px Arial';
        ctx.fillText(channel.name, width - 65, yOffset + 14);
      }

      ctx.fillStyle = '#666';
      ctx.font = '8px Arial';
      if (autoScaleY && channelData.length > 0) {
        const allValues = channelData.map(d => d.values[channelIdx] * channel.gain + channel.offset);
        const minVal = Math.min(...allValues);
        const maxVal = Math.max(...allValues);
        ctx.fillText(maxVal.toFixed(2), 3, yOffset + 10);
        ctx.fillText(minVal.toFixed(2), 3, yOffset + channelHeight - 4);
      } else {
        ctx.fillText(yMax.toFixed(2), 3, yOffset + 10);
        ctx.fillText(yMin.toFixed(2), 3, yOffset + channelHeight - 4);
      }
    }

    ctx.fillStyle = '#666';
    ctx.font = '8px Arial';
    ctx.fillText('0', padding - 8, height - 4);
    ctx.fillText(String(plotWidth), width - 25, height - 4);

  }, [channelData, channels, autoScaleY, yMin, yMax, plotWidth]);

  return (
    <div className="bg-white rounded-lg shadow-sm border border-slate-200 p-3">
      <div className="flex items-center justify-between mb-2.5">
        <div className="flex items-center gap-2">
          <div className="p-1.5 bg-blue-50 rounded">
            <Activity className="w-5 h-5 text-blue-600" />
          </div>
          <div>
            <h2 className="text-sm font-semibold text-slate-800">Multi-Channel Plot</h2>
            <p className="text-xs text-slate-500">Serial Plot View</p>
          </div>
        </div>
        <button
          onClick={clearChannelData}
          className="flex items-center gap-1 px-2.5 py-1.5 bg-slate-100 text-slate-700 text-xs rounded hover:bg-slate-200 transition-colors"
        >
          <Trash2 className="w-3 h-3" />
          Clear
        </button>
      </div>

      <div className="flex flex-wrap gap-2 mb-2.5">
        <div className="flex items-center gap-1.5">
          <label className="text-xs text-slate-600">Buffer:</label>
          <input
            type="number"
            value={bufferSize}
            onChange={(e) => setBufferSize(Number(e.target.value))}
            className="w-20 px-2 py-1 text-xs border border-slate-300 rounded"
          />
        </div>
        <div className="flex items-center gap-1.5">
          <label className="text-xs text-slate-600">Width:</label>
          <input
            type="number"
            value={plotWidth}
            onChange={(e) => setPlotWidth(Number(e.target.value))}
            className="w-20 px-2 py-1 text-xs border border-slate-300 rounded"
          />
        </div>
        <div className="flex items-center gap-1.5">
          <label className="text-xs text-slate-600">Auto Y:</label>
          <input
            type="checkbox"
            checked={autoScaleY}
            onChange={(e) => setAutoScaleY(e.target.checked)}
            className="w-3.5 h-3.5 rounded"
          />
        </div>
        {!autoScaleY && (
          <>
            <div className="flex items-center gap-1.5">
              <label className="text-xs text-slate-600">Y Min:</label>
              <input
                type="number"
                step="0.1"
                value={yMin}
                onChange={(e) => setYMin(Number(e.target.value))}
                className="w-16 px-2 py-1 text-xs border border-slate-300 rounded"
              />
            </div>
            <div className="flex items-center gap-1.5">
              <label className="text-xs text-slate-600">Y Max:</label>
              <input
                type="number"
                step="0.1"
                value={yMax}
                onChange={(e) => setYMax(Number(e.target.value))}
                className="w-16 px-2 py-1 text-xs border border-slate-300 rounded"
              />
            </div>
          </>
        )}
        <div className="flex gap-1.5 ml-auto">
          <button
            onClick={showAllChannels}
            className="px-2.5 py-1 bg-blue-100 text-blue-700 text-xs rounded hover:bg-blue-200"
          >
            Show All
          </button>
          <button
            onClick={hideAllChannels}
            className="px-2.5 py-1 bg-blue-100 text-blue-700 text-xs rounded hover:bg-blue-200"
          >
            Hide All
          </button>
        </div>
      </div>

      <div className="bg-slate-50 rounded p-2 mb-2.5">
        <canvas
          ref={canvasRef}
          width={700}
          height={280}
          className="w-full rounded border border-slate-200"
        />
      </div>

      <div className="bg-slate-50 rounded overflow-x-auto">
        <table className="w-full text-xs">
          <thead>
            <tr className="bg-slate-100">
              <th className="px-2 py-1.5 text-left font-medium">Channel</th>
              <th className="px-2 py-1.5 text-center font-medium">Visible</th>
              <th className="px-2 py-1.5 text-center font-medium">Gain</th>
              <th className="px-2 py-1.5 text-center font-medium">Offset</th>
            </tr>
          </thead>
          <tbody>
            {channels.map((channel) => (
              <tr key={channel.id} className="border-t border-slate-200 hover:bg-slate-100">
                <td className="px-2 py-1.5">
                  <span 
                    className="inline-block w-2.5 h-2.5 rounded-full mr-1.5"
                    style={{ backgroundColor: channel.color }}
                  />
                  {channel.name}
                </td>
                <td className="px-2 py-1.5 text-center">
                  <button
                    onClick={() => toggleChannelVisibility(channel.id)}
                    className="p-0.5 hover:bg-slate-200 rounded"
                  >
                    {channel.visible ? (
                      <Eye className="w-3.5 h-3.5 text-green-600" />
                    ) : (
                      <EyeOff className="w-3.5 h-3.5 text-slate-400" />
                    )}
                  </button>
                </td>
                <td className="px-2 py-1.5 text-center">
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
                    className="w-14 px-1.5 py-0.5 text-center text-xs border border-slate-300 rounded"
                  />
                </td>
                <td className="px-2 py-1.5 text-center">
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
                    className="w-14 px-1.5 py-0.5 text-center text-xs border border-slate-300 rounded"
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
