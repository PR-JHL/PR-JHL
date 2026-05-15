import React, { useState } from 'react';
import { Bluetooth, Search, CheckCircle, Activity, RefreshCw } from 'lucide-react';
import { useAppStore, BluetoothDevice as DeviceType } from '../store/useAppStore';

const mockDevices: DeviceType[] = [
  { id: '1', name: 'Pacemaker-001', rssi: -65, connected: false },
  { id: '2', name: 'Pacemaker-002', rssi: -78, connected: false },
  { id: '3', name: 'Lead-Monitor-A', rssi: -55, connected: false },
];

export const BluetoothPanel: React.FC = () => {
  const { connectedDevice, isConnected, setConnectedDevice, setConnected } = useAppStore();
  const [scanning, setScanning] = useState(false);
  const [devices] = useState<DeviceType[]>(mockDevices);

  const handleScan = () => {
    setScanning(true);
    setTimeout(() => {
      setScanning(false);
    }, 2000);
  };

  const handleConnect = (device: DeviceType) => {
    setConnectedDevice(device);
    setConnected(true);
  };

  const handleDisconnect = () => {
    setConnectedDevice(null);
    setConnected(false);
  };

  const getSignalStrength = (rssi: number) => {
    if (rssi >= -60) return { level: 'Strong', color: 'text-green-500', bars: 4 };
    if (rssi >= -70) return { level: 'Medium', color: 'text-yellow-500', bars: 3 };
    if (rssi >= -80) return { level: 'Weak', color: 'text-orange-500', bars: 2 };
    return { level: 'Very Weak', color: 'text-red-500', bars: 1 };
  };

  return (
    <div className="bg-white rounded-lg shadow-sm border border-slate-200 p-3">
      <div className="flex items-center justify-between mb-3">
        <div className="flex items-center gap-2">
          <div className="p-1.5 bg-blue-50 rounded">
            <Bluetooth className="w-5 h-5 text-blue-600" />
          </div>
          <div>
            <h2 className="text-sm font-semibold text-slate-800">Bluetooth</h2>
            <p className="text-xs text-slate-500">Connect Device</p>
          </div>
        </div>
        <div className="flex gap-1.5">
          <button
            onClick={handleScan}
            disabled={scanning || isConnected}
            className="flex items-center gap-1 px-2.5 py-1.5 bg-blue-600 text-white text-xs rounded hover:bg-blue-700 transition-colors disabled:opacity-50"
          >
            {scanning ? <RefreshCw className="w-3 h-3 animate-spin" /> : <Search className="w-3 h-3" />}
            {scanning ? 'Scanning' : 'Scan'}
          </button>
          <button
            onClick={() => handleConnect(mockDevices[0])}
            disabled={isConnected}
            className="flex items-center gap-1 px-2.5 py-1.5 bg-green-600 text-white text-xs rounded hover:bg-green-700 transition-colors disabled:opacity-50"
          >
            <Activity className="w-3 h-3" />
            Connect
          </button>
        </div>
      </div>

      {isConnected && connectedDevice ? (
        <div className="space-y-2">
          <div className="flex items-center justify-between p-2.5 bg-green-50 rounded border border-green-200">
            <div className="flex items-center gap-2">
              <CheckCircle className="w-5 h-5 text-green-500" />
              <div>
                <p className="text-sm font-medium text-slate-800">{connectedDevice.name}</p>
                <p className="text-xs text-green-600">Connected</p>
              </div>
            </div>
            <button
              onClick={handleDisconnect}
              className="px-2.5 py-1 bg-red-500 text-white text-xs rounded hover:bg-red-600 transition-colors"
            >
              Disconnect
            </button>
          </div>
          <div className="grid grid-cols-2 gap-2">
            <div className="p-2 bg-slate-50 rounded">
              <p className="text-xs text-slate-500">Signal</p>
              <p className="text-sm font-semibold text-slate-800">
                {getSignalStrength(connectedDevice.rssi).level}
              </p>
            </div>
            <div className="p-2 bg-slate-50 rounded">
              <p className="text-xs text-slate-500">RSSI</p>
              <p className="text-sm font-semibold text-slate-800">{connectedDevice.rssi} dBm</p>
            </div>
          </div>
        </div>
      ) : (
        <div className="space-y-2">
          {devices.map((device) => {
            const signal = getSignalStrength(device.rssi);
            return (
              <div
                key={device.id}
                onClick={() => handleConnect(device)}
                className="flex items-center justify-between p-2.5 bg-slate-50 rounded hover:bg-slate-100 cursor-pointer transition-colors border border-slate-200"
              >
                <div className="flex items-center gap-2.5">
                  <Activity className="w-4 h-4 text-blue-600" />
                  <div>
                    <p className="text-sm font-medium text-slate-800">{device.name}</p>
                    <p className="text-xs text-slate-500">{device.rssi} dBm</p>
                  </div>
                </div>
                <div className="flex items-center gap-1.5">
                  <div className="flex gap-0.5">
                    {[...Array(4)].map((_, i) => (
                      <div
                        key={i}
                        className={`w-1 rounded-full ${
                          i < signal.bars ? signal.color : 'bg-slate-300'
                        }`}
                        style={{ height: `${(i + 1) * 4}px` }}
                      />
                    ))}
                  </div>
                  <span className={`text-xs ${signal.color}`}>{signal.level}</span>
                </div>
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
};
