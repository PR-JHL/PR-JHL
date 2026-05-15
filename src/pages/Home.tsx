import React from 'react';
import { BluetoothPanel } from '../components/BluetoothPanel';
import { MultiChannelPlot } from '../components/MultiChannelPlot';
import { CommandPanel } from '../components/CommandPanel';
import { Activity } from 'lucide-react';

export const Home: React.FC = () => {
  return (
    <div className="min-h-screen bg-slate-50">
      <header className="bg-white border-b border-slate-200 px-4 py-2">
        <div className="flex items-center gap-2">
          <div className="p-1.5 bg-blue-50 rounded-lg">
            <Activity className="w-6 h-6 text-blue-600" />
          </div>
          <div>
            <h1 className="text-lg font-bold text-slate-800">Cardiac Lead Testing System</h1>
            <p className="text-xs text-slate-500">Bluetooth & Piezo Module Communication</p>
          </div>
        </div>
      </header>

      <main className="p-4">
        <div className="grid grid-cols-1 lg:grid-cols-12 gap-4">
          <div className="lg:col-span-3 space-y-4">
            <BluetoothPanel />
            <CommandPanel />
          </div>
          <div className="lg:col-span-9">
            <MultiChannelPlot />
          </div>
        </div>
      </main>
    </div>
  );
};
