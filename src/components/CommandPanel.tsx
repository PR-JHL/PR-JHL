import React, { useState } from 'react';
import { Send, Play, Square, RotateCcw, Activity, Zap, Settings, History } from 'lucide-react';
import { useAppStore } from '../store/useAppStore';

const presetCommands = [
  { name: 'Start Test', payload: 'START_TEST', icon: Play },
  { name: 'Stop Test', payload: 'STOP_TEST', icon: Square },
  { name: 'Reset', payload: 'RESET', icon: RotateCcw },
  { name: 'Calibrate', payload: 'CALIBRATE', icon: Activity },
  { name: 'Pulse Test', payload: 'PULSE_TEST', icon: Zap },
  { name: 'Get Status', payload: 'GET_STATUS', icon: Settings },
];

export const CommandPanel: React.FC = () => {
  const { isConnected, addCommand, commandHistory } = useAppStore();
  const [customCommand, setCustomCommand] = useState('');

  const handleSendCommand = (name: string, payload: string) => {
    if (!isConnected) return;
    addCommand({
      id: Date.now().toString(),
      name,
      payload,
      timestamp: Date.now(),
    });
  };

  const handleSendCustom = () => {
    if (!customCommand.trim() || !isConnected) return;
    handleSendCommand('Custom', customCommand);
    setCustomCommand('');
  };

  return (
    <div className="bg-white rounded-lg shadow-sm border border-slate-200 p-3">
      <div className="flex items-center gap-2 mb-3">
        <div className="p-1.5 bg-blue-50 rounded">
          <Send className="w-5 h-5 text-blue-600" />
        </div>
        <div>
          <h2 className="text-sm font-semibold text-slate-800">Commands</h2>
          <p className="text-xs text-slate-500">Send Commands</p>
        </div>
      </div>

      <div className="grid grid-cols-3 gap-1.5 mb-3">
        {presetCommands.map((cmd) => {
          const Icon = cmd.icon;
          return (
            <button
              key={cmd.name}
              onClick={() => handleSendCommand(cmd.name, cmd.payload)}
              disabled={!isConnected}
              className="flex flex-col items-center gap-1 p-2 bg-slate-50 rounded hover:bg-blue-50 hover:border-blue-200 border border-slate-200 transition-all disabled:opacity-50"
            >
              <Icon className="w-4 h-4 text-blue-600" />
              <span className="text-xs font-medium text-slate-700">{cmd.name}</span>
            </button>
          );
        })}
      </div>

      <div className="mb-3">
        <p className="text-xs font-medium text-slate-700 mb-1.5">Custom Command</p>
        <div className="flex gap-1.5">
          <input
            type="text"
            value={customCommand}
            onChange={(e) => setCustomCommand(e.target.value)}
            placeholder="Enter command..."
            disabled={!isConnected}
            className="flex-1 px-2 py-1.5 text-xs border border-slate-300 rounded focus:outline-none focus:ring-1.5 focus:ring-blue-500 disabled:opacity-50"
            onKeyPress={(e) => e.key === 'Enter' && handleSendCustom()}
          />
          <button
            onClick={handleSendCustom}
            disabled={!isConnected || !customCommand.trim()}
            className="px-3 py-1.5 bg-blue-600 text-white text-xs rounded hover:bg-blue-700 transition-colors disabled:opacity-50"
          >
            Send
          </button>
        </div>
      </div>

      <div>
        <div className="flex items-center gap-1.5 mb-2">
          <History className="w-3.5 h-3.5 text-slate-500" />
          <p className="text-xs font-medium text-slate-700">History</p>
        </div>
        <div className="max-h-24 overflow-y-auto space-y-1.5">
          {commandHistory.length === 0 ? (
            <p className="text-xs text-slate-400 text-center py-2">No records</p>
          ) : (
            commandHistory.slice(0, 5).map((cmd) => (
              <div
                key={cmd.id}
                className="flex items-center justify-between p-2 bg-slate-50 rounded border border-slate-200"
              >
                <div>
                  <p className="text-xs font-medium text-slate-700">{cmd.name}</p>
                  <p className="text-xs text-slate-500 font-mono truncate max-w-[100px]">{cmd.payload}</p>
                </div>
                <p className="text-xs text-slate-400">
                  {new Date(cmd.timestamp).toLocaleTimeString()}
                </p>
              </div>
            ))
          )}
        </div>
      </div>
    </div>
  );
};
