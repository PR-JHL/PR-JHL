import { create } from 'zustand';

export interface BluetoothDevice {
  id: string;
  name: string;
  rssi: number;
  connected: boolean;
}

export interface ChannelData {
  timestamp: number;
  values: number[];
}

export interface Channel {
  id: number;
  name: string;
  color: string;
  visible: boolean;
  gain: number;
  offset: number;
}

export interface Command {
  id: string;
  name: string;
  payload: string;
  timestamp: number;
}

export const CHANNELS: Channel[] = [
  { id: 1, name: 'Ch1', color: '#e74c3c', visible: true, gain: 1, offset: 0 },
  { id: 2, name: 'Ch2', color: '#3498db', visible: true, gain: 1, offset: 0 },
  { id: 3, name: 'Ch3', color: '#2ecc71', visible: true, gain: 1, offset: 0 },
  { id: 4, name: 'Ch4', color: '#f39c12', visible: true, gain: 1, offset: 0 },
];

interface AppState {
  connectedDevice: BluetoothDevice | null;
  channelData: ChannelData[];
  channels: Channel[];
  commandHistory: Command[];
  isConnected: boolean;
  bufferSize: number;
  setConnectedDevice: (device: BluetoothDevice | null) => void;
  addChannelData: (data: ChannelData) => void;
  addCommand: (command: Command) => void;
  setConnected: (connected: boolean) => void;
  clearChannelData: () => void;
  toggleChannelVisibility: (channelId: number) => void;
  setBufferSize: (size: number) => void;
}

export const useAppStore = create<AppState>((set) => ({
  connectedDevice: null,
  channelData: [],
  channels: CHANNELS,
  commandHistory: [],
  isConnected: false,
  bufferSize: 1000,
  setConnectedDevice: (device) => set({ connectedDevice: device }),
  addChannelData: (data) => set((state) => ({
    channelData: [...state.channelData.slice(-(state.bufferSize - 1)), data]
  })),
  addCommand: (command) => set((state) => ({
    commandHistory: [command, ...state.commandHistory.slice(49)]
  })),
  setConnected: (connected) => set({ isConnected: connected }),
  clearChannelData: () => set({ channelData: [] }),
  toggleChannelVisibility: (channelId) => set((state) => ({
    channels: state.channels.map(ch => 
      ch.id === channelId ? { ...ch, visible: !ch.visible } : ch
    )
  })),
  setBufferSize: (size) => set({ bufferSize: size }),
}));
