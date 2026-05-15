import { create } from 'zustand';

export interface BluetoothDevice {
  id: string;
  name: string;
  rssi: number;
  connected: boolean;
}

export interface SensorData {
  timestamp: number;
  value: number;
  voltage: number;
}

export interface Command {
  id: string;
  name: string;
  payload: string;
  timestamp: number;
}

interface AppState {
  connectedDevice: BluetoothDevice | null;
  sensorData: SensorData[];
  commandHistory: Command[];
  isConnected: boolean;
  setConnectedDevice: (device: BluetoothDevice | null) => void;
  addSensorData: (data: SensorData) => void;
  addCommand: (command: Command) => void;
  setConnected: (connected: boolean) => void;
  clearSensorData: () => void;
}

export const useAppStore = create<AppState>((set) => ({
  connectedDevice: null,
  sensorData: [],
  commandHistory: [],
  isConnected: false,
  setConnectedDevice: (device) => set({ connectedDevice: device }),
  addSensorData: (data) => set((state) => ({
    sensorData: [...state.sensorData.slice(-499), data]
  })),
  addCommand: (command) => set((state) => ({
    commandHistory: [command, ...state.commandHistory.slice(49)]
  })),
  setConnected: (connected) => set({ isConnected: connected }),
  clearSensorData: () => set({ sensorData: [] }),
}));
