import React from 'react';
import { BluetoothPanel } from '../components/BluetoothPanel';
import { DataVisualization } from '../components/DataVisualization';
import { CommandPanel } from '../components/CommandPanel';
import { Activity } from 'lucide-react';

export const Home: React.FC = () => {
  return (
    <div className="min-h-screen bg-slate-50">
      <header className="bg-white border-b border-slate-200 px-8 py-4">
        <div className="flex items-center gap-3">
          <div className="p-2 bg-blue-50 rounded-lg">
            <Activity className="w-8 h-8 text-blue-600" />
          </div>
          <div>
            <h1 className="text-2xl font-bold text-slate-800">心脏起搏导线检测系统</h1>
            <p className="text-sm text-slate-500">蓝牙与压电模块通信界面</p>
          </div>
        </div>
      </header>

      <main className="p-8">
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          <div className="lg:col-span-1 space-y-6">
            <BluetoothPanel />
            <CommandPanel />
          </div>
          <div className="lg:col-span-2">
            <DataVisualization />
          </div>
        </div>
      </main>
    </div>
  );
};
