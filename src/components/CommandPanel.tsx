import React, { useState } from 'react';
import { useAppStore } from '../store/useAppStore';

const commands = [
  { name: 'Start', payload: 'START' },
  { name: 'Stop', payload: 'STOP' },
  { name: 'Reset', payload: 'RESET' },
];

export const CommandPanel: React.FC = () => {
  const { isConnected, addCommand } = useAppStore();
  const [customCmd, setCustomCmd] = useState('');

  const sendCmd = (name: string, payload: string) => {
    if (!isConnected) return;
    addCommand({
      id: Date.now().toString(),
      name,
      payload,
      timestamp: Date.now(),
    });
  };

  const sendCustom = () => {
    if (!customCmd.trim() || !isConnected) return;
    sendCmd('Custom', customCmd);
    setCustomCmd('');
  };

  return (
    <div className="border border-gray-300 bg-white p-3">
      <h3 className="text-sm font-bold text-gray-700 mb-2">Commands</h3>
      
      <div className="grid grid-cols-3 gap-1 mb-2">
        {commands.map((cmd) => (
          <button
            key={cmd.name}
            onClick={() => sendCmd(cmd.name, cmd.payload)}
            disabled={!isConnected}
            className="text-xs bg-gray-200 px-2 py-1 rounded hover:bg-gray-300 disabled:opacity-50"
          >
            {cmd.name}
          </button>
        ))}
      </div>

      <div className="flex gap-1">
        <input
          type="text"
          value={customCmd}
          onChange={(e) => setCustomCmd(e.target.value)}
          placeholder="Cmd..."
          disabled={!isConnected}
          className="flex-1 text-xs border border-gray-300 px-2 py-1 rounded disabled:opacity-50"
          onKeyPress={(e) => e.key === 'Enter' && sendCustom()}
        />
        <button
          onClick={sendCustom}
          disabled={!isConnected || !customCmd.trim()}
          className="text-xs bg-blue-600 text-white px-2 py-1 rounded hover:bg-blue-700 disabled:opacity-50"
        >
          Send
        </button>
      </div>
    </div>
  );
};
