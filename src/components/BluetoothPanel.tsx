import React, { useState } from 'react';
import { useAppStore, BluetoothDevice as DeviceType } from '../store/useAppStore';

const mockDevices: DeviceType[] = [
  { id: '1', name: 'Device-1', rssi: -65, connected: false },
  { id: '2', name: 'Device-2', rssi: -78, connected: false },
];

export const BluetoothPanel: React.FC = () => {
  const { connectedDevice, isConnected, setConnectedDevice, setConnected } = useAppStore();
  const [scanning, setScanning] = useState(false);

  const handleScan = () => {
    setScanning(true);
    setTimeout(() => setScanning(false), 1500);
  };

  const handleConnect = (device: DeviceType) => {
    setConnectedDevice(device);
    setConnected(true);
  };

  const handleDisconnect = () => {
    setConnectedDevice(null);
    setConnected(false);
  };

  return (
    <div className="border border-gray-300 bg-white p-3">
      <h3 className="text-sm font-bold text-gray-700 mb-2">Bluetooth</h3>
      
      <button
        onClick={handleScan}
        disabled={scanning || isConnected}
        className="w-full text-xs bg-gray-200 px-3 py-1.5 rounded mb-2 hover:bg-gray-300 disabled:opacity-50"
      >
        {scanning ? 'Scanning...' : 'Scan'}
      </button>

      <button
        onClick={() => handleConnect(mockDevices[0])}
        disabled={isConnected}
        className="w-full text-xs bg-green-600 text-white px-3 py-1.5 rounded mb-2 hover:bg-green-700 disabled:opacity-50"
      >
        Connect
      </button>

      {isConnected && connectedDevice && (
        <div className="mb-2">
          <div className="text-xs text-green-600 mb-1">Connected: {connectedDevice.name}</div>
          <button
            onClick={handleDisconnect}
            className="w-full text-xs bg-red-500 text-white px-3 py-1.5 rounded hover:bg-red-600"
          >
            Disconnect
          </button>
        </div>
      )}

      {!isConnected && (
        <div className="text-xs text-gray-500">
          {mockDevices.map((d) => (
            <div key={d.id} className="py-1 border-b border-gray-200">
              {d.name} ({d.rssi} dBm)
            </div>
          ))}
        </div>
      )}
    </div>
  );
};
