import React from 'react';
import { BluetoothPanel } from '../components/BluetoothPanel';
import { MultiChannelPlot } from '../components/MultiChannelPlot';
import { CommandPanel } from '../components/CommandPanel';

export const Home: React.FC = () => {
  return (
    <div className="min-h-screen bg-gray-100 p-4">
      <h1 className="text-lg font-bold text-gray-800 mb-4">Serial Plot</h1>
      
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div className="md:col-span-1 space-y-4">
          <BluetoothPanel />
          <CommandPanel />
        </div>
        <div className="md:col-span-3">
          <MultiChannelPlot />
        </div>
      </div>
    </div>
  );
};
