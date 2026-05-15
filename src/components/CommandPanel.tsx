import React, { useState } from 'react';
import { Send, Play, Square, RotateCcw, Activity, Zap, Settings, History } from 'lucide-react';
import { useAppStore } from '../store/useAppStore';

const presetCommands = [
  { name: '开始测试', payload: 'START_TEST', icon: Play },
  { name: '停止测试', payload: 'STOP_TEST', icon: Square },
  { name: '重置设备', payload: 'RESET', icon: RotateCcw },
  { name: '校准传感器', payload: 'CALIBRATE', icon: Activity },
  { name: '脉冲测试', payload: 'PULSE_TEST', icon: Zap },
  { name: '获取状态', payload: 'GET_STATUS', icon: Settings },
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
    handleSendCommand('自定义指令', customCommand);
    setCustomCommand('');
  };

  return (
    <div className="bg-white rounded-xl shadow-md border border-slate-200 p-6">
      <div className="flex items-center justify-between mb-6">
        <div className="flex items-center gap-3">
          <div className="p-2 bg-blue-50 rounded-lg">
            <Send className="w-6 h-6 text-blue-600" />
          </div>
          <div>
            <h2 className="text-lg font-semibold text-slate-800">指令控制</h2>
            <p className="text-sm text-slate-500">发送测试指令到设备</p>
          </div>
        </div>
      </div>

      <div className="grid grid-cols-3 gap-3 mb-6">
        {presetCommands.map((cmd) => {
          const Icon = cmd.icon;
          return (
            <button
              key={cmd.name}
              onClick={() => handleSendCommand(cmd.name, cmd.payload)}
              disabled={!isConnected}
              className="flex flex-col items-center gap-2 p-4 bg-slate-50 rounded-lg hover:bg-blue-50 hover:border-blue-200 border border-slate-200 transition-all disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <Icon className="w-6 h-6 text-blue-600" />
              <span className="text-sm font-medium text-slate-700">{cmd.name}</span>
            </button>
          );
        })}
      </div>

      <div className="mb-6">
        <p className="text-sm font-medium text-slate-700 mb-2">自定义指令</p>
        <div className="flex gap-2">
          <input
            type="text"
            value={customCommand}
            onChange={(e) => setCustomCommand(e.target.value)}
            placeholder="输入指令..."
            disabled={!isConnected}
            className="flex-1 px-4 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent disabled:opacity-50"
            onKeyPress={(e) => e.key === 'Enter' && handleSendCustom()}
          />
          <button
            onClick={handleSendCustom}
            disabled={!isConnected || !customCommand.trim()}
            className="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          >
            发送
          </button>
        </div>
      </div>

      <div>
        <div className="flex items-center gap-2 mb-3">
          <History className="w-4 h-4 text-slate-500" />
          <p className="text-sm font-medium text-slate-700">发送历史</p>
        </div>
        <div className="max-h-48 overflow-y-auto space-y-2">
          {commandHistory.length === 0 ? (
            <p className="text-sm text-slate-400 text-center py-4">暂无发送记录</p>
          ) : (
            commandHistory.map((cmd) => (
              <div
                key={cmd.id}
                className="flex items-center justify-between p-3 bg-slate-50 rounded-lg border border-slate-200"
              >
                <div>
                  <p className="text-sm font-medium text-slate-700">{cmd.name}</p>
                  <p className="text-xs text-slate-500 font-mono">{cmd.payload}</p>
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
