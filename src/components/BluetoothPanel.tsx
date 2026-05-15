import React, { useState } from 'react';
import { Bluetooth, Search, CheckCircle, XCircle, Activity, RefreshCw } from 'lucide-react';
import { useAppStore, BluetoothDevice as DeviceType } from '../store/useAppStore';

const mockDevices: DeviceType[] = [
  { id: '1', name: 'Pacemaker-001', rssi: -65, connected: false },
  { id: '2', name: 'Pacemaker-002', rssi: -78, connected: false },
  { id: '3', name: 'Lead-Monitor-A', rssi: -55, connected: false },
];

export const BluetoothPanel: React.FC = () => {
  const { connectedDevice, isConnected, setConnectedDevice, setConnected } = useAppStore();
  const [scanning, setScanning] = useState(false);
  const [devices, setDevices] = useState<DeviceType[]>(mockDevices);

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
    if (rssi >= -60) return { level: '强', color: 'text-green-500', bars: 4 };
    if (rssi >= -70) return { level: '中', color: 'text-yellow-500', bars: 3 };
    if (rssi >= -80) return { level: '弱', color: 'text-orange-500', bars: 2 };
    return { level: '很弱', color: 'text-red-500', bars: 1 };
  };

  return (
    <div className="bg-white rounded-xl shadow-md border border-slate-200 p-6">
      <div className="flex items-center justify-between mb-6">
        <div className="flex items-center gap-3">
          <div className="p-2 bg-blue-50 rounded-lg">
            <Bluetooth className="w-6 h-6 text-blue-600" />
          </div>
          <div>
            <h2 className="text-lg font-semibold text-slate-800">蓝牙连接</h2>
            <p className="text-sm text-slate-500">连接心脏起搏导线检测设备</p>
          </div>
        </div>
        <button
          onClick={handleScan}
          disabled={scanning}
          className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors disabled:opacity-50"
        >
          {scanning ? <RefreshCw className="w-4 h-4 animate-spin" /> : <Search className="w-4 h-4" />}
          {scanning ? '扫描中...' : '扫描设备'}
        </button>
      </div>

      {isConnected && connectedDevice ? (
        <div className="space-y-4">
          <div className="flex items-center justify-between p-4 bg-green-50 rounded-lg border border-green-200">
            <div className="flex items-center gap-3">
              <CheckCircle className="w-8 h-8 text-green-500" />
              <div>
                <p className="font-medium text-slate-800">{connectedDevice.name}</p>
                <p className="text-sm text-green-600">已连接</p>
              </div>
            </div>
            <button
              onClick={handleDisconnect}
              className="px-4 py-2 bg-red-500 text-white rounded-lg hover:bg-red-600 transition-colors"
            >
              断开连接
            </button>
          </div>
          <div className="grid grid-cols-2 gap-4">
            <div className="p-4 bg-slate-50 rounded-lg">
              <p className="text-sm text-slate-500 mb-1">信号强度</p>
              <p className="text-lg font-semibold text-slate-800">
                {getSignalStrength(connectedDevice.rssi).level}
              </p>
            </div>
            <div className="p-4 bg-slate-50 rounded-lg">
              <p className="text-sm text-slate-500 mb-1">RSSI</p>
              <p className="text-lg font-semibold text-slate-800">{connectedDevice.rssi} dBm</p>
            </div>
          </div>
        </div>
      ) : (
        <div className="space-y-3">
          {devices.map((device) => {
            const signal = getSignalStrength(device.rssi);
            return (
              <div
                key={device.id}
                onClick={() => handleConnect(device)}
                className="flex items-center justify-between p-4 bg-slate-50 rounded-lg hover:bg-slate-100 cursor-pointer transition-colors border border-slate-200"
              >
                <div className="flex items-center gap-4">
                  <Activity className="w-5 h-5 text-blue-600" />
                  <div>
                    <p className="font-medium text-slate-800">{device.name}</p>
                    <p className="text-sm text-slate-500">{device.rssi} dBm</p>
                  </div>
                </div>
                <div className="flex items-center gap-2">
                  <div className="flex gap-1">
                    {[...Array(4)].map((_, i) => (
                      <div
                        key={i}
                        className={`w-1.5 rounded-full ${
                          i < signal.bars ? signal.color : 'bg-slate-300'
                        }`}
                        style={{ height: `${(i + 1) * 6}px` }}
                      />
                    ))}
                  </div>
                  <span className={`text-sm ${signal.color}`}>{signal.level}</span>
                </div>
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
};
