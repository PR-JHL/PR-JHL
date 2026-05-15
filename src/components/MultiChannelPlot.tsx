import React, { useEffect, useRef } from 'react';
import { useAppStore } from '../store/useAppStore';

export const MultiChannelPlot: React.FC = () => {
  const { channelData, channels, isConnected, addChannelData, clearChannelData } = useAppStore();
  const canvasRef = useRef<HTMLCanvasElement>(null);

  useEffect(() => {
    if (!isConnected) return;

    const interval = setInterval(() => {
      const timestamp = Date.now();
      const values = Array(4).fill(0).map((_, i) => {
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

    const channelHeight = height / 4;
    const padding = 35;
    const plotWidth = width - padding;

    for (let ch = 0; ch < 4; ch++) {
      const channel = channels[ch];
      if (!channel.visible) continue;

      const yOffset = ch * channelHeight;

      ctx.strokeStyle = '#e0e0e0';
      ctx.lineWidth = 1;
      for (let i = 0; i <= 4; i++) {
        const y = yOffset + (channelHeight / 4) * i;
        ctx.beginPath();
        ctx.moveTo(padding, y);
        ctx.lineTo(width, y);
        ctx.stroke();
      }

      for (let i = 0; i <= 5; i++) {
        const x = padding + (plotWidth / 5) * i;
        ctx.beginPath();
        ctx.moveTo(x, yOffset + 2);
        ctx.lineTo(x, yOffset + channelHeight - 2);
        ctx.stroke();
      }

      if (channelData.length > 1) {
        const data = channelData.slice(-500);
        
        ctx.beginPath();
        ctx.strokeStyle = channel.color;
        ctx.lineWidth = 1.5;

        data.forEach((d, idx) => {
          const x = padding + (idx / (data.length - 1)) * plotWidth;
          const y = yOffset + channelHeight / 2 + d.values[ch] * 50;
          
          if (idx === 0) {
            ctx.moveTo(x, y);
          } else {
            ctx.lineTo(x, y);
          }
        });
        ctx.stroke();

        ctx.fillStyle = channel.color;
        ctx.font = '10px Arial';
        ctx.fillText(channel.name, width - 30, yOffset + 14);
      }

      ctx.fillStyle = '#666';
      ctx.font = '8px Arial';
      ctx.fillText('0', padding - 8, yOffset + channelHeight / 2 + 4);
    }

    ctx.fillText('0', padding - 8, height - 4);
    ctx.fillText('500', width - 25, height - 4);

  }, [channelData, channels]);

  return (
    <div className="border border-gray-300 bg-white p-3">
      <div className="flex justify-between items-center mb-2">
        <h3 className="text-sm font-bold text-gray-700">Plot</h3>
        <button
          onClick={clearChannelData}
          className="text-xs bg-gray-200 px-2 py-1 rounded hover:bg-gray-300"
        >
          Clear
        </button>
      </div>

      <div className="bg-gray-100 p-2">
        <canvas
          ref={canvasRef}
          width={600}
          height={200}
          className="w-full border border-gray-300"
        />
      </div>

      <div className="mt-2">
        <table className="w-full text-xs">
          <thead>
            <tr className="bg-gray-100">
              <th className="px-2 py-1 text-left">Ch</th>
              <th className="px-2 py-1 text-center">Color</th>
            </tr>
          </thead>
          <tbody>
            {channels.map((ch) => (
              <tr key={ch.id} className="border-t border-gray-200">
                <td className="px-2 py-1">{ch.name}</td>
                <td className="px-2 py-1">
                  <span 
                    className="inline-block w-4 h-4 rounded"
                    style={{ backgroundColor: ch.color }}
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
